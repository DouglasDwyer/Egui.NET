using System.Collections.Immutable;

namespace Egui;

public partial record struct AtomLayoutResponse
{
    public IEnumerable<(Id, Rect)> CustomRects => EguiMarshal.Call<AtomLayoutResponse, ImmutableArray<(Id, Rect)>>(EguiFn.egui_atomics_atom_layout_AtomLayoutResponse_custom_rects, this);
}