using System.Collections.Immutable;

namespace Egui;

public partial record struct Style
{
    /// <summary>
    /// The default text styles of the default egui theme.
    /// </summary>
    public static ImmutableArray<(TextStyle Style, FontId Font)> DefaultTextStyles()
    {
        return EguiMarshal.Call<ImmutableArray<(TextStyle, FontId)>>(EguiFn.egui_style_default_text_styles);
    }
}
