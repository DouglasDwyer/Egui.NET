namespace Egui.Epaint.Text;

/// <summary>
/// Helpers for working with byte ranges into UTF-8 text. See <see cref="Egui.Text.ByteIndex"/>.
/// </summary>
public static class ByteRange
{
    /// <summary>
    /// The full byte range covering <paramref name="text"/>, i.e. <c>0..text.Length</c>.
    /// </summary>
    public static (Egui.Text.ByteIndex Start, Egui.Text.ByteIndex End) Full(string text)
    {
        return EguiMarshal.Call<string, (Egui.Text.ByteIndex, Egui.Text.ByteIndex)>(EguiFn.epaint_text_index_ByteRange_full, text);
    }

    /// <summary>
    /// Slices the given string by this byte range.
    /// </summary>
    public static string Slice(string text, Egui.Text.ByteIndex start, Egui.Text.ByteIndex end)
    {
        return EguiMarshal.Call<string, Egui.Text.ByteIndex, Egui.Text.ByteIndex, string>(EguiFn.epaint_text_index_ByteRange_slice, text, start, end);
    }
}

/// <summary>
/// Helpers for working with character ranges into text. See <see cref="Egui.Text.CharIndex"/>.
/// </summary>
public static class CharRange
{
    /// <summary>
    /// The full character range covering <paramref name="text"/>, i.e. <c>0..text.Chars().Count()</c>.
    /// </summary>
    public static (Egui.Text.CharIndex Start, Egui.Text.CharIndex End) Full(string text)
    {
        return EguiMarshal.Call<string, (Egui.Text.CharIndex, Egui.Text.CharIndex)>(EguiFn.epaint_text_index_CharRange_full, text);
    }
}
