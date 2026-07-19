namespace Egui.Epaint.Text;

/// <summary>
/// Helpers for working with byte offsets into UTF-8 text.<br/>
///
/// <c>ByteIndex</c> is a <c>#[serde(transparent)]</c> newtype around <c>usize</c> on the Rust
/// side, so it has no dedicated C# type of its own - offsets are passed around as plain
/// <see cref="ulong"/>s instead.
/// </summary>
public static class ByteIndex
{
    /// <summary>
    /// The zero offset, i.e. the very start of the text.
    /// </summary>
    public static ulong Default()
    {
        return EguiMarshal.Call<ulong>(EguiFn.epaint_text_index_ByteIndex_default);
    }

    /// <summary>
    /// Saturating integer addition.
    /// </summary>
    public static ulong SaturatingAdd(ulong index, ulong rhs)
    {
        return EguiMarshal.Call<ulong, ulong, ulong>(EguiFn.epaint_text_index_ByteIndex_saturating_add, index, rhs);
    }

    /// <summary>
    /// Saturating integer subtraction.
    /// </summary>
    public static ulong SaturatingSub(ulong index, ulong rhs)
    {
        return EguiMarshal.Call<ulong, ulong, ulong>(EguiFn.epaint_text_index_ByteIndex_saturating_sub, index, rhs);
    }
}

/// <summary>
/// Helpers for working with character (Unicode scalar) offsets into text.<br/>
///
/// <c>CharIndex</c> is a <c>#[serde(transparent)]</c> newtype around <c>usize</c> on the Rust
/// side, so it has no dedicated C# type of its own - offsets are passed around as plain
/// <see cref="ulong"/>s instead.
/// </summary>
public static class CharIndex
{
    /// <summary>
    /// The zero offset, i.e. the very start of the text.
    /// </summary>
    public static ulong Default()
    {
        return EguiMarshal.Call<ulong>(EguiFn.epaint_text_index_CharIndex_default);
    }

    /// <summary>
    /// Saturating integer addition.
    /// </summary>
    public static ulong SaturatingAdd(ulong index, ulong rhs)
    {
        return EguiMarshal.Call<ulong, ulong, ulong>(EguiFn.epaint_text_index_CharIndex_saturating_add, index, rhs);
    }

    /// <summary>
    /// Saturating integer subtraction.
    /// </summary>
    public static ulong SaturatingSub(ulong index, ulong rhs)
    {
        return EguiMarshal.Call<ulong, ulong, ulong>(EguiFn.epaint_text_index_CharIndex_saturating_sub, index, rhs);
    }
}

/// <summary>
/// Helpers for working with byte ranges into UTF-8 text. See <see cref="ByteIndex"/>.
/// </summary>
public static class ByteRange
{
    /// <summary>
    /// The full byte range covering <paramref name="text"/>, i.e. <c>0..text.Length</c>.
    /// </summary>
    public static (ulong Start, ulong End) Full(string text)
    {
        return EguiMarshal.Call<string, (ulong, ulong)>(EguiFn.epaint_text_index_ByteRange_full, text);
    }

    /// <summary>
    /// Slices the given string by this byte range.
    /// </summary>
    public static string Slice(string text, ulong start, ulong end)
    {
        return EguiMarshal.Call<string, ulong, ulong, string>(EguiFn.epaint_text_index_ByteRange_slice, text, start, end);
    }
}

/// <summary>
/// Helpers for working with character ranges into text. See <see cref="CharIndex"/>.
/// </summary>
public static class CharRange
{
    /// <summary>
    /// The full character range covering <paramref name="text"/>, i.e. <c>0..text.Chars().Count()</c>.
    /// </summary>
    public static (ulong Start, ulong End) Full(string text)
    {
        return EguiMarshal.Call<string, (ulong, ulong)>(EguiFn.epaint_text_index_CharRange_full, text);
    }
}
