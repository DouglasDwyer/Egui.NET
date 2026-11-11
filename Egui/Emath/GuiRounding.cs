namespace Egui.Emath;

/// <summary>
/// Rounds coordinates and sizes to a multiple of the GUI rounding factor or of the physical pixel size.
/// This mirrors egui's <c>GuiRounding</c> trait for <see cref="float"/> and <see cref="double"/>.
/// </summary>
public static class GuiRounding
{
    /// <summary>
    /// We (sometimes) round sizes and coordinates to an even multiple of this value.
    /// </summary>
    public const float GuiRoundingFactor = 1.0f / 32.0f;

    /// <summary>
    /// Rounds to an even multiple of <see cref="GuiRoundingFactor"/>.
    /// Use this for widget coordinates and sizes, which are in logical UI points.
    /// </summary>
    public static float RoundUi(float value) => MathF.Round(value / GuiRoundingFactor, MidpointRounding.AwayFromZero) * GuiRoundingFactor;

    /// <inheritdoc cref="RoundUi(float)"/>
    public static double RoundUi(double value) => Math.Round(value / GuiRoundingFactor, MidpointRounding.AwayFromZero) * GuiRoundingFactor;

    /// <summary>
    /// Rounds a size or position, given in logical UI points, to an even multiple of the physical pixel size.
    /// <paramref name="pixelsPerPoint"/> is the number of physical pixels per logical point, e.g. <c>2</c> on a high-DPI screen.
    /// </summary>
    public static float RoundToPixels(float value, float pixelsPerPoint) => MathF.Round(value * pixelsPerPoint, MidpointRounding.AwayFromZero) / pixelsPerPoint;

    /// <inheritdoc cref="RoundToPixels(float, float)"/>
    public static double RoundToPixels(double value, float pixelsPerPoint) => Math.Round(value * pixelsPerPoint, MidpointRounding.AwayFromZero) / pixelsPerPoint;
}
