using System.ComponentModel;

namespace Egui.EguiPlot;

/// <summary>
/// A series of values forming a path.
/// </summary>
/// <remarks>
/// `egui_plot::PlotPoints` can't cross the FFI boundary (one of its variants stores a closure),
/// so this always carries a plain point list; the closure-based constructors
/// (`from_explicit_callback`/`from_parametric_callback`) aren't exposed. `Line::gradient_color`
/// (also a closure) isn't exposed for the same reason.
/// </remarks>
public struct Line
{
    private string _name;
    private (double X, double Y)[] _points;
    private Stroke? _stroke;
    private float? _fill;
    private float? _fillAlpha;
    private LineStyle? _style;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("'Line' does not contain a constructor that takes 0 arguments", error: true)]
    public Line() { throw new InvalidOperationException(); }

    /// <summary>
    /// Creates a new line with the given name (used in the legend) and points.
    /// </summary>
    public Line(string name, IEnumerable<(double X, double Y)> points)
    {
        _name = name;
        _points = points.ToArray();
        _stroke = null;
        _fill = null;
        _fillAlpha = null;
        _style = null;
    }

    /// <summary>
    /// Add a stroke.
    /// </summary>
    public readonly Line Stroke(Stroke stroke)
    {
        var result = this;
        result._stroke = stroke;
        return result;
    }

    /// <summary>
    /// Fill the area between this line and a horizontal reference line.
    /// </summary>
    public readonly Line Fill(float yReference)
    {
        var result = this;
        result._fill = yReference;
        return result;
    }

    /// <summary>
    /// Set the fill's alpha channel, which blends with the stroke color. Default: <c>0.05</c>.
    /// </summary>
    public readonly Line FillAlpha(float alpha)
    {
        var result = this;
        result._fillAlpha = alpha;
        return result;
    }

    /// <summary>
    /// Set the line's style. Default: <see cref="LineStyle.Solid"/>.
    /// </summary>
    public readonly Line Style(LineStyle style)
    {
        var result = this;
        result._style = style;
        return result;
    }

    internal static void Serialize(BincodeSerializer serializer, Line value) => value.Serialize(serializer);

    internal readonly void Serialize(BincodeSerializer serializer)
    {
        serializer.increase_container_depth();
        serializer.serialize_str(_name);
        serializer.serialize_len(_points.Length);
        foreach (var (x, y) in _points)
        {
            serializer.serialize_f64(x);
            serializer.serialize_f64(y);
        }
        TraitHelpers.serialize_option_Stroke(_stroke, serializer);
        TraitHelpers.serialize_option_f32(_fill, serializer);
        TraitHelpers.serialize_option_f32(_fillAlpha, serializer);
        TraitHelpers.serialize_option_LineStyle(_style, serializer);
        serializer.decrease_container_depth();
    }

    internal static Line Deserialize(BincodeDeserializer deserializer)
    {
        deserializer.increase_container_depth();
        Line obj = default;
        obj._name = deserializer.deserialize_str();
        long length = deserializer.deserialize_len();
        obj._points = new (double, double)[length];
        for (long i = 0; i < length; i++)
        {
            obj._points[i] = (deserializer.deserialize_f64(), deserializer.deserialize_f64());
        }
        obj._stroke = TraitHelpers.deserialize_option_Stroke(deserializer);
        obj._fill = TraitHelpers.deserialize_option_f32(deserializer);
        obj._fillAlpha = TraitHelpers.deserialize_option_f32(deserializer);
        obj._style = TraitHelpers.deserialize_option_LineStyle(deserializer);
        deserializer.decrease_container_depth();
        return obj;
    }
}
