namespace Egui.TextSelection;

public static partial class TextSelectionHelpers
{
    /// <summary>
    /// Slices <paramref name="text"/> by a character range.
    /// </summary>
    public static string SliceCharRange(string text, Egui.Text.CharIndex start, Egui.Text.CharIndex end)
    {
        return EguiMarshal.Call<string, Egui.Text.CharIndex, Egui.Text.CharIndex, string>(EguiFn.egui_text_selection_text_cursor_state_slice_char_range, text, start, end);
    }
}
