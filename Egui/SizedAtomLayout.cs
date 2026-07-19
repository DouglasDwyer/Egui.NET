using System.Collections.Immutable;

namespace Egui;

public partial struct SizedAtomLayout
{
    /// <summary>
    /// The auto-generated <c>_sizedAtoms</c> field is private (it mirrors the private Rust
    /// field), so this internal property is the only way for other types in this assembly -
    /// namely <see cref="AllocatedAtomLayout"/>'s <c>Images</c>/<c>Texts</c>/<c>Kinds</c>/
    /// <c>MapKind</c>/<c>MapImages</c> - to read or replace the sized atoms.
    /// </summary>
    internal ImmutableArray<SizedAtom> SizedAtoms
    {
        readonly get => _sizedAtoms;
        set => _sizedAtoms = value;
    }
}
