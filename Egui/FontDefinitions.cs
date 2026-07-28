using System.Collections.Immutable;

namespace Egui;

public partial record struct FontDefinitions
{
    /// <summary>
    /// Lists all of the builtin font names used by <c>epaint</c>.
    /// </summary>
    public IEnumerable<string> BuiltinFontNames => EguiMarshal.Call<ImmutableArray<string>>(EguiFn.epaint_text_fonts_FontDefinitions_builtin_font_names);
}