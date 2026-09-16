namespace Egui.Epaint.ShapeTransform;

public static partial class ShapeTransformHelpers
{
    /// <summary>
    /// Remember to handle <see cref="Color32.Placeholder"/> specially!
    /// </summary>
    public unsafe static void AdjustColors(ref Shape shape, MutateDelegate<Color32> adjustColor)
    {
        using var callback = new EguiCallback.Pin(colorPtr =>
        {
            var color = *(Color32*)colorPtr;
            adjustColor(ref color);
            *(Color32*)colorPtr = color;
        });
        shape = EguiMarshal.Call<Shape, EguiCallback, Shape>(EguiFn.epaint_shape_transform_adjust_colors, shape, callback.AsNative());
    }
}