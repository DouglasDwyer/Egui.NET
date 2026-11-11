# C# unions for Rust enums

Enums with at least one payload variant (35 types, including mixed unit and payload enums) are emitted as C# 15 custom union types. Fieldless enums stay C# `enum`s.

## Generated shape

```csharp
[System.Runtime.CompilerServices.Union]
public partial record struct TextureId : System.Runtime.CompilerServices.IUnion
{
    private int? _variantId;
    private Managed _variant0;
    private User _variant1;

    public TextureId(Managed value) { _variantId = 0; _variant0 = value; }
    public TextureId(User value) { _variantId = 1; _variant1 = value; }

    public readonly object? Value { get; }
    public readonly bool HasValue { get; }
    public readonly bool TryGetValue(out Managed value);
    public readonly bool TryGetValue(out User value);

    public partial record struct Managed { public ulong Value; }
    public partial record struct User { public ulong Value; }
}
```

- The unions are `record struct`s following the basic union pattern and the non-boxing access pattern. Type patterns call `TryGetValue` and `null` patterns call `HasValue`, so neither boxes.
- Each variant keeps its own field, as before. `_variantId` is the serde variant index, so gaps in the indices are preserved. The bincode wire format is unchanged.
- Case types are the nested `partial record struct`s, with the same names and public fields as before. Union conversions replace the generated implicit operators.
- `Inner` remains as an `[Obsolete]` shim returning `Value`.

## Initialization

`default(T)` is an uninitialized union (`_variantId == null`).

- **Rust type has a `Default`:** the bindgen already binds a public parameterless constructor to it. An uninitialized union behaves as that value, created once through the constructor in a private `DefaultHolder`. Currently `AtomKind`, `ColorMode`, `FontColorTransferFunction`, `FontFamily`, `FontSelection`, `HintingTarget`, `PlotPoints`, `SizeHint`, `SizedAtomKind`, `TextureId` and `WidgetText`.
- **Otherwise:** `new T()` is a compile error (the existing `[Obsolete(error: true)]` constructor). Reading an uninitialized union (`Value`, `HasValue`, `TryGetValue`, serialization) throws `InvalidOperationException`. Equality, `GetHashCode` and `ToString` still work.

## Where the changes are

| Area | Change |
| --- | --- |
| `serde-generate/src/csharp.rs` | `output_enum_container` emits the union. `output_cstyle_enum` is unchanged. |
| `serde-generate/src/config.rs` | New `enums_with_default` option. |
| `egui_net_bindgen/src/lib.rs` | Computes `enums_with_default` from the types with a bound parameterless constructor. |
| `Egui/Atoms.cs`, `Egui/SizedAtomLayout.cs`, `Egui/Widgets/ImageSource.cs`, `Egui.Silk.NET.OpenGL/SilkGlIntegration.cs` | `.Inner` replaced by pattern matching. |
| `Egui.Tests` | Tests for conversion, pattern matching, equality, the default fallback (including through a Rust call), and the throwing behavior. |
| csprojs, CI, `README.md` | Target `net11.0` and the .NET 11 SDK. |

## Migration

| Before | After |
| --- | --- |
| `x.Inner is Foo.Bar` | `x is Foo.Bar` |
| `((Foo.Bar)x.Inner).Value` | `x.TryGetValue(out Foo.Bar bar)` |
| `switch (x.Inner) { case Foo.Bar b: ... }` | `switch (x) { case Foo.Bar b: ... }` |

## Possible follow-ups

- Overlap unmanaged payloads, or box rare large cases. Large unions such as `ViewportCommand` (36 variants) and `Event` (17) are as large as before.
- Pass large case types by `in` in the union constructors.
