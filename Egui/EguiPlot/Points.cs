using System.ComponentModel;

namespace Egui.EguiPlot;

/// <summary>
/// A set of points.
/// </summary>
/// <remarks>
/// `egui_plot::PlotPoints` can't cross the FFI boundary (one of its variants stores a closure),
/// so this always carries a plain point list; the closure-based constructors
/// (`from_explicit_callback`/`from_parametric_callback`) aren't exposed.
/// </remarks>
public struct Points
{
    private string _name;
    private (double X, double Y)[] _points;
    private MarkerShape? _shape;
    private Color32? _color;
    private bool? _filled;
    private float? _radius;
    private float? _stems;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("'Points' does not contain a constructor that takes 0 arguments", error: true)]
    public Points() { throw new InvalidOperationException(); }

    /// <summary>
    /// Creates a new set of points with the given name (used in the legend) and positions.
    /// </summary>
    public Points(string name, IEnumerable<(double X, double Y)> points)
    {
        _name = name;
        _points = points.ToArray();
        _shape = null;
        _color = null;
        _filled = null;
        _radius = null;
        _stems = null;
    }

    /// <summary>
    /// Set the marker's shape.
    /// </summary>
    public readonly Points Shape(MarkerShape shape)
    {
        var result = this;
        result._shape = shape;
        return result;
    }

    /// <summary>
    /// Set the marker's color. <see cref="Color32.Transparent"/> means it will be picked automatically.
    /// </summary>
    public readonly Points Color(Color32 color)
    {
        var result = this;
        result._color = color;
        return result;
    }

    /// <summary>
    /// Whether to fill the marker.
    /// </summary>
    public readonly Points Filled(bool filled)
    {
        var result = this;
        result._filled = filled;
        return result;
    }

    /// <summary>
    /// Set the maximum extent of the marker around its center.
    /// </summary>
    public readonly Points Radius(float radius)
    {
        var result = this;
        result._radius = radius;
        return result;
    }

    /// <summary>
    /// Draw a stem for each marker, from the given y reference down/up to the marker.
    /// </summary>
    public readonly Points Stems(float yReference)
    {
        var result = this;
        result._stems = yReference;
        return result;
    }

    internal static void Serialize(BincodeSerializer serializer, Points value) => value.Serialize(serializer);

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
        TraitHelpers.serialize_option_MarkerShape(_shape, serializer);
        TraitHelpers.serialize_option_Color32(_color, serializer);
        TraitHelpers.serialize_option_bool(_filled, serializer);
        TraitHelpers.serialize_option_f32(_radius, serializer);
        TraitHelpers.serialize_option_f32(_stems, serializer);
        serializer.decrease_container_depth();
    }

    internal static Points Deserialize(BincodeDeserializer deserializer)
    {
        deserializer.increase_container_depth();
        Points obj = default;
        obj._name = deserializer.deserialize_str();
        long length = deserializer.deserialize_len();
        obj._points = new (double, double)[length];
        for (long i = 0; i < length; i++)
        {
            obj._points[i] = (deserializer.deserialize_f64(), deserializer.deserialize_f64());
        }
        obj._shape = TraitHelpers.deserialize_option_MarkerShape(deserializer);
        obj._color = TraitHelpers.deserialize_option_Color32(deserializer);
        obj._filled = TraitHelpers.deserialize_option_bool(deserializer);
        obj._radius = TraitHelpers.deserialize_option_f32(deserializer);
        obj._stems = TraitHelpers.deserialize_option_f32(deserializer);
        deserializer.decrease_container_depth();
        return obj;
    }
}
