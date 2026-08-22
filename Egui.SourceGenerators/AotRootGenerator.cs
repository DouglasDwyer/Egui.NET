using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Egui.SourceGenerators;

/// <summary>
/// Roots every closed generic instantiation that <c>EguiMarshal.SerializerCache&lt;T&gt;</c>
/// dispatches to reflectively (<c>ImmutableArray&lt;T&gt;</c>, <c>Nullable&lt;T&gt;</c>, tuples,
/// <c>ArrayN&lt;T&gt;</c>), so Native AOT compiles them ahead of time instead of only ever
/// discovering them through <see cref="System.Reflection.MethodInfo.MakeGenericMethod"/>.
/// </summary>
/// <remarks>
/// Finds every <c>EguiMarshal.Call&lt;...&gt;</c> and <c>EguiMarshal.SerializerCache&lt;...&gt;</c>
/// usage in the compilation via the semantic model, and emits a module initializer that
/// ordinarily references each closed instantiation through <c>EguiMarshal.AotRoot</c>.
/// </remarks>
[Generator(LanguageNames.CSharp)]
public sealed class AotRootGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var typeArgumentLists = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (node, _) => node is GenericNameSyntax { Identifier.ValueText: "Call" or "SerializerCache" },
                transform: static (ctx, ct) => ResolveTypeArguments((GenericNameSyntax)ctx.Node, ctx.SemanticModel, ct))
            .Where(static x => !x.IsDefaultOrEmpty)
            .Collect();

        context.RegisterSourceOutput(typeArgumentLists.Combine(context.CompilationProvider),
            static (spc, data) => Emit(spc, data.Left, data.Right));
    }

    /// <summary>
    /// If <paramref name="node"/> is an <c>EguiMarshal.Call&lt;...&gt;</c> or
    /// <c>EguiMarshal.SerializerCache&lt;...&gt;</c> reference, returns its type arguments.
    /// </summary>
    private static ImmutableArray<ITypeSymbol> ResolveTypeArguments(GenericNameSyntax node, SemanticModel model, CancellationToken ct)
    {
        if (node.Identifier.ValueText == "Call")
        {
            // The generic name is the `Name` of a member access that's the target of an
            // invocation - resolve the invocation to get the bound method and its type
            // arguments, rather than trying to bind the generic name in isolation.
            if (node.Parent is not MemberAccessExpressionSyntax { Parent: InvocationExpressionSyntax invocation }
                || model.GetSymbolInfo(invocation, ct).Symbol is not IMethodSymbol { ContainingType.Name: "EguiMarshal" } method)
            {
                return ImmutableArray<ITypeSymbol>.Empty;
            }

            return method.TypeArguments;
        }

        // `EguiMarshal.SerializerCache<T>` is a type reference, not a method call.
        if (model.GetSymbolInfo(node, ct).Symbol is not INamedTypeSymbol { ContainingType.Name: "EguiMarshal" } namedType)
        {
            return ImmutableArray<ITypeSymbol>.Empty;
        }

        return namedType.TypeArguments;
    }

    /// <summary>
    /// Reads <c>EguiMarshal.SerializerPrototypes</c>'s <c>typeof(...)</c> keys directly, so the
    /// set of wrapper shapes this generator roots is never out of sync with the set
    /// <c>SerializerCache&lt;T&gt;</c> actually dispatches reflectively.
    /// </summary>
    private static ImmutableArray<INamedTypeSymbol> ResolveWrapperShapes(Compilation compilation)
    {
        var field = compilation.GetTypeByMetadataName("Egui.EguiMarshal")?
            .GetMembers("SerializerPrototypes").OfType<IFieldSymbol>().FirstOrDefault();

        if (field?.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is not VariableDeclaratorSyntax { Initializer: { } initializer })
        {
            return ImmutableArray<INamedTypeSymbol>.Empty;
        }

        var model = compilation.GetSemanticModel(initializer.SyntaxTree);
        var shapes = ImmutableArray.CreateBuilder<INamedTypeSymbol>();
        foreach (var typeOfExpression in initializer.DescendantNodes().OfType<TypeOfExpressionSyntax>())
        {
            if (model.GetSymbolInfo(typeOfExpression.Type).Symbol is INamedTypeSymbol symbol)
            {
                shapes.Add(symbol.OriginalDefinition);
            }
        }
        return shapes.ToImmutable();
    }

    /// <summary>
    /// Recursively decomposes <paramref name="type"/>, recording an <see cref="AotRoot"/> for
    /// every closed instantiation of a type in <paramref name="shapes"/> found within it.
    /// </summary>
    private static void CollectRoots(ITypeSymbol type, ImmutableArray<INamedTypeSymbol> shapes, List<AotRoot> roots)
    {
        if (type is not INamedTypeSymbol { IsGenericType: true } named)
        {
            return;
        }

        // `(A, B)` tuple literal syntax and the explicit `ValueTuple<A, B>` spelling resolve to
        // distinct-but-equivalent symbols; normalize to the underlying type so both match.
        if (named.IsTupleType)
        {
            named = named.TupleUnderlyingType ?? named;
        }

        if (shapes.Contains(named.OriginalDefinition, SymbolEqualityComparer.Default))
        {
            roots.Add(new AotRoot(named.OriginalDefinition.Name, string.Join(", ", named.TypeArguments.Select(FullyQualify))));
            foreach (var typeArgument in named.TypeArguments)
            {
                CollectRoots(typeArgument, shapes, roots);
            }
        }

        // Otherwise this is a plain primitive, a generated struct/enum with its own non-generic
        // Serialize/Deserialize methods, or an unsupported compound type - nothing to root.
    }

    private static string FullyQualify(ITypeSymbol type) => type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    private static void Emit(SourceProductionContext context, ImmutableArray<ImmutableArray<ITypeSymbol>> typeArgumentLists, Compilation compilation)
    {
        var shapes = ResolveWrapperShapes(compilation);

        var roots = new List<AotRoot>();
        foreach (var typeArguments in typeArgumentLists)
        {
            foreach (var typeArgument in typeArguments)
            {
                CollectRoots(typeArgument, shapes, roots);
            }
        }

        var distinctRoots = roots.Distinct()
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
    /// A single closed instantiation of a wrapper shape, e.g. (<c>"ImmutableArray"</c>,
    /// <c>"global::Egui.Color32"</c>) or (<c>"ValueTuple"</c>, <c>"bool, string"</c>).
    /// </summary>
    private readonly record struct AotRoot(string Shape, string TypeArguments);
}
