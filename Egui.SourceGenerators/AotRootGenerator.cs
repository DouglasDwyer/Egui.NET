using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Egui.SourceGenerators;

/// <summary>
/// <c>EguiMarshal.SerializerCache&lt;T&gt;</c> dispatches to a small, fixed set of compound
/// generic wrapper serializers (<c>ImmutableArray&lt;T&gt;</c>, <c>Nullable&lt;T&gt;</c>,
/// tuples, <c>ArrayN&lt;T&gt;</c>) via <see cref="System.Reflection.MethodInfo.MakeGenericMethod"/>.
/// Native AOT cannot create new generic method instantiations at runtime, so every closed
/// instantiation that the bindings use has to be referenced *ordinarily* (not through
/// reflection) somewhere in the compiled program ahead of time.
/// </summary>
/// <remarks>
/// This generator finds every <c>EguiMarshal.Call&lt;...&gt;</c> and
/// <c>EguiMarshal.SerializerCache&lt;...&gt;</c> usage in the compilation (both generated
/// bindings and hand-written code - they're part of the same compilation, so no separate
/// scan of the hand-written source tree is needed), recursively decomposes each compound
/// generic argument using the semantic model, and emits a companion file with a module
/// initializer that ordinarily references every instantiation found via
/// <c>EguiMarshal.AotRoot</c>, forcing the AOT compiler to compile them ahead of time.
/// </remarks>
[Generator(LanguageNames.CSharp)]
public sealed class AotRootGenerator : IIncrementalGenerator
{
    /// <summary>
    /// The metadata names of the well-known "wrapper" generic type definitions that
    /// <c>EguiMarshal.SerializerCache&lt;T&gt;</c> dispatches to reflectively, paired with the
    /// name of the corresponding rooting method on <c>EguiMarshal.AotRoot</c>.
    /// </summary>
    private static readonly (string MetadataName, string Shape)[] WrapperShapes =
    [
        ("System.Collections.Immutable.ImmutableArray`1", "ImmutableArray"),
        ("System.Nullable`1", "Nullable"),
        ("DouglasDwyer.FixedArray.Array2`1", "Array2"),
        ("DouglasDwyer.FixedArray.Array3`1", "Array3"),
        ("DouglasDwyer.FixedArray.Array4`1", "Array4"),
        ("DouglasDwyer.FixedArray.Array5`1", "Array5"),
        ("DouglasDwyer.FixedArray.Array6`1", "Array6"),
        ("System.ValueTuple`2", "Tuple2"),
        ("System.ValueTuple`3", "Tuple3"),
        ("System.ValueTuple`4", "Tuple4"),
    ];

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var roots = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (node, _) => node is GenericNameSyntax { Identifier.ValueText: "Call" or "SerializerCache" },
                transform: static (ctx, ct) => CollectFromNode((GenericNameSyntax)ctx.Node, ctx.SemanticModel, ct))
            .SelectMany(static (found, _) => found);

        context.RegisterSourceOutput(roots.Collect(), static (spc, found) => Emit(spc, found));
    }

    /// <summary>
    /// If <paramref name="node"/> is an <c>EguiMarshal.Call&lt;...&gt;</c> or
    /// <c>EguiMarshal.SerializerCache&lt;...&gt;</c> reference, resolves its type arguments and
    /// recursively decomposes each into <see cref="AotRoot"/>s.
    /// </summary>
    private static ImmutableArray<AotRoot> CollectFromNode(GenericNameSyntax node, SemanticModel model, CancellationToken ct)
    {
        ImmutableArray<ITypeSymbol> typeArguments;

        if (node.Identifier.ValueText == "Call")
        {
            // `EguiMarshal.Call<...>(...)`: the generic name is the `Name` of a member access
            // that's itself the target of an invocation - resolve the invocation to get the
            // bound method (and its inferred/explicit type arguments) rather than trying to
            // bind the generic name in isolation.
            if (node.Parent is not MemberAccessExpressionSyntax { Parent: InvocationExpressionSyntax invocation })
            {
                return ImmutableArray<AotRoot>.Empty;
            }

            if (model.GetSymbolInfo(invocation, ct).Symbol is not IMethodSymbol { ContainingType.Name: "EguiMarshal" } method
                || method.TypeArguments.Length == 0)
            {
                return ImmutableArray<AotRoot>.Empty;
            }

            typeArguments = method.TypeArguments;
        }
        else
        {
            // `EguiMarshal.SerializerCache<T>`: a type reference, not a method call.
            if (model.GetSymbolInfo(node, ct).Symbol is not INamedTypeSymbol { ContainingType.Name: "EguiMarshal" } namedType
                || namedType.TypeArguments.Length == 0)
            {
                return ImmutableArray<AotRoot>.Empty;
            }

            typeArguments = namedType.TypeArguments;
        }

        var shapes = ResolveWrapperShapes(model.Compilation);

        var roots = new List<AotRoot>();
        foreach (var typeArgument in typeArguments)
        {
            CollectRoots(typeArgument, shapes, roots);
        }
        return [.. roots];
    }

    /// <summary>
    /// Resolves the symbol for each entry in <see cref="WrapperShapes"/> against
    /// <paramref name="compilation"/>. A symbol is <see langword="null"/> if that shape isn't
    /// referenced anywhere reachable from the compilation (e.g. the <c>DouglasDwyer.FixedArray</c>
    /// package reference is missing) - such shapes simply never match in <see cref="CollectRoots"/>.
    /// </summary>
    private static (INamedTypeSymbol? Symbol, string Shape)[] ResolveWrapperShapes(Compilation compilation) =>
        [.. WrapperShapes.Select(x => (compilation.GetTypeByMetadataName(x.MetadataName), x.Shape))];

    /// <summary>
    /// Recursively decomposes <paramref name="type"/> and records an <see cref="AotRoot"/> for
    /// every compound generic wrapper shape (see <see cref="WrapperShapes"/>) found within it.
    /// </summary>
    private static void CollectRoots(ITypeSymbol type, (INamedTypeSymbol? Symbol, string Shape)[] shapes, List<AotRoot> roots)
    {
        if (type is not INamedTypeSymbol { IsGenericType: true } named)
        {
            return;
        }

        // C#'s `(A, B)` tuple literal syntax and the explicit `ValueTuple<A, B>` spelling
        // resolve to distinct-but-equivalent symbols; normalize to the underlying type so
        // both are recognized identically.
        if (named.IsTupleType)
        {
            named = named.TupleUnderlyingType ?? named;
        }

        foreach (var (symbol, shape) in shapes)
        {
            if (symbol is null || !SymbolEqualityComparer.Default.Equals(named.OriginalDefinition, symbol))
            {
                continue;
            }

            roots.Add(new AotRoot(shape, string.Join(", ", named.TypeArguments.Select(FullyQualify))));
            foreach (var typeArgument in named.TypeArguments)
            {
                CollectRoots(typeArgument, shapes, roots);
            }
            return;
        }

        // A plain primitive or generated struct/enum type (or an unsupported compound type,
        // e.g. `ImmutableDictionary<,>`) - not a wrapper shape `SerializerCache<T>` needs to
        // root; it either has no generic instantiation to create, or is handled through its
        // own non-generic `Serialize`/`Deserialize` methods.
    }

    private static string FullyQualify(ITypeSymbol type) => type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    private static void Emit(SourceProductionContext context, ImmutableArray<AotRoot> found)
    {
        var distinctRoots = found.Distinct()
            .OrderBy(static x => x.Shape, StringComparer.Ordinal)
            .ThenBy(static x => x.TypeArguments, StringComparer.Ordinal);

        var body = new StringBuilder();
        foreach (var root in distinctRoots)
        {
            body.Append("        global::Egui.EguiMarshal.AotRoot.").Append(root.Shape)
                .Append('<').Append(root.TypeArguments).Append(">();\n");
        }

        var source = $$"""
            // <auto-generated/>
            #pragma warning disable

            // Roots every closed generic instantiation of EguiMarshal's compound-type serializers
            // that the compilation actually uses, so that Native AOT compiles them ahead of time
            // instead of only ever discovering them through reflection. See the remarks on
            // EguiMarshal.SerializerCache<T> and EguiMarshal.AotRoot for details.
            internal static class EguiMarshalAotRoots
            {
                [System.Runtime.CompilerServices.ModuleInitializer]
                internal static void Init()
                {
            {{body}}    }
            }

            """;

        context.AddSource("EguiMarshalAotRoots.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    /// <summary>
    /// A single closed instantiation of one of <see cref="WrapperShapes"/>, e.g.
    /// (<c>"ImmutableArray"</c>, <c>"global::Egui.Color32"</c>) or
    /// (<c>"Tuple2"</c>, <c>"global::System.Boolean, global::System.String"</c>).
    /// </summary>
    private readonly record struct AotRoot(string Shape, string TypeArguments);
}
