namespace Egui;

public partial struct RichText
{
    /// <summary>
    /// Creates a <see cref="RichText"/> object for the provided string.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    public static implicit operator RichText(string value) => new RichText(value);

    /// <summary>
    /// Sets a variable font axis, e.g. <c>"wght"</c> for weight, to the given coordinate.
    /// </summary>
    /// <param name="tag">The 4-character variation axis tag.</param>
    /// <param name="coord">The coordinate to set the axis to.</param>
    public readonly RichText Variation(string tag, float coord)
    {
        return EguiMarshal.Call<RichText, string, float, RichText>(EguiFn.egui_widget_text_RichText_variation, this, tag, coord);
    }

    /// <summary>
    /// Sets a variable font axis to the given coordinate, using a big-endian-packed <c>u32</c> tag
    /// (e.g. <c>0x77676874</c> for <c>"wght"</c>).
    /// </summary>
    /// <param name="tag">The variation axis tag.</param>
    /// <param name="coord">The coordinate to set the axis to.</param>
    public readonly RichText Variation(uint tag, float coord)
    {
        return EguiMarshal.Call<RichText, uint, float, RichText>(EguiFn.egui_widget_text_RichText_variation_u32, this, tag, coord);
    }

    /// <summary>
    /// Sets a variable font axis, e.g. <c>"wght"</c> for weight, to the given coordinate.
    /// </summary>
    /// <param name="tag">The 4-byte variation axis tag.</param>
    /// <param name="coord">The coordinate to set the axis to.</param>
    public readonly RichText Variation(byte[] tag, float coord)
    {
        if (tag.Length != 4)
        {
            throw new ArgumentException("Variation tag must be exactly 4 bytes.", nameof(tag));
        }

        var packed = new Array4<byte>(tag[0], tag[1], tag[2], tag[3]);
        return EguiMarshal.Call<RichText, Array4<byte>, float, RichText>(EguiFn.egui_widget_text_RichText_variation_bytes, this, packed, coord);
    }
}