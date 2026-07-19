namespace Egui.TextSelection;

/// <summary>
/// Free functions for converting between byte and character offsets into text, from
/// <c>egui::text_selection::text_cursor_state</c>.
/// </summary>
public static class TextCursorStateHelpers
{
    /// <summary>
    /// Converts a character offset into a byte offset into <paramref name="text"/>.
    /// </summary>
    public static ulong ByteIndexFromCharIndex(string text, ulong charIndex)
    {
        return EguiMarshal.Call<string, ulong, ulong>(EguiFn.egui_text_selection_text_cursor_state_byte_index_from_char_index, text, charIndex);
    }

    /// <summary>
    /// Converts a byte offset into a character offset into <paramref name="text"/>.
    /// </summary>
    public static ulong CharIndexFromByteIndex(string text, ulong byteIndex)
    {
        return EguiMarshal.Call<string, ulong, ulong>(EguiFn.egui_text_selection_text_cursor_state_char_index_from_byte_index, text, byteIndex);
    }

    /// <summary>
    /// Slices <paramref name="text"/> by a character range.
    /// </summary>
    public static string SliceCharRange(string text, ulong start, ulong end)
    {
        return EguiMarshal.Call<string, ulong, ulong, string>(EguiFn.egui_text_selection_text_cursor_state_slice_char_range, text, start, end);
    }
}
