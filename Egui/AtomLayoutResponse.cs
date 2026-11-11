using System.Collections.Immutable;

namespace Egui;

public partial record struct AtomLayoutResponse
{
    public IEnumerable<(IdSalt, Rect)> CustomRects => EguiMarshal.Call<AtomLayoutResponse, ImmutableArray<(IdSalt, Rect)>>(EguiFn.egui_atomics_widget_atom_WidgetAtomResponse_custom_rects, this);
}