using System.Collections.Immutable;

namespace Egui;

public partial record struct FontData
{
    /// <summary>
    /// Wraps the content of a <c>.ttf</c> or <c>.otf</c> file, and uses the font face at <paramref name="index"/>.
    /// When in doubt, use <c>0</c>.<br/>
    ///
    /// This sets the font bytes directly, without copying them through Rust.
    /// </summary>
    public static FontData FromBlob(ImmutableArray<byte> font, uint index)
    {
        FontData result = default;
        result.Font = font;
        result.Index = index;
        result.Tweak = new FontTweak();
        return result;
    }

    /// <summary>
    /// Copies the content of a <c>.ttf</c> or <c>.otf</c> file, and uses its first font face.
    /// </summary>
    public static FontData FromStatic(ReadOnlySpan<byte> font) => FromBlob(ImmutableArray.Create(font), 0);
}
