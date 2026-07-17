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
}