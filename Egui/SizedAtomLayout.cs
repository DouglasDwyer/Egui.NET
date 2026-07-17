using System.Collections.Immutable;

namespace Egui;

public partial struct SizedAtomLayout
{
    internal ImmutableArray<SizedAtom> SizedAtoms
    {
        readonly get => _sizedAtoms;
        set => _sizedAtoms = value;
    }
}
