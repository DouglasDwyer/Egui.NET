namespace Egui;

public partial record struct Shape
{
    /// <summary>
    /// Creates a gradient rectangle that transitions from <paramref name="from"/> to <paramref name="to"/>
    /// along the given <paramref name="direction"/>.
    /// </summary>
    /// <param name="rect">The rectangle to paint.</param>
    /// <param name="direction">The direction of the gradient.</param>
    /// <param name="from">The color at the start edge.</param>
    /// <param name="to">The color at the end edge.</param>
    public static Shape GradientRect(Egui.Rect rect, Direction direction, Color32 from, Color32 to)
    {
        return EguiMarshal.Call<Egui.Rect, Direction, Color32, Color32, Shape>(EguiFn.epaint_shapes_shape_Shape_gradient_rect, rect, direction, from, to);
    }
}
