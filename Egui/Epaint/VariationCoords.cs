using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Egui.Epaint.Text;

public partial record struct VariationCoords
{
    /// <summary>
    /// Creates a list of variation coordinates from a sequence of (tag, value) pairs.
    /// </summary>
    public static VariationCoords New(IEnumerable<(Tag Tag, float Coord)> values)
    {
        return EguiMarshal.Call<ImmutableArray<(Tag, float)>, VariationCoords>(EguiFn.epaint_text_text_layout_types_VariationCoords_new, values.ToImmutableArray());
    }
}
