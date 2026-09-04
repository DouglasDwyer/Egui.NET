using System.ComponentModel;

namespace Egui.EguiPlot;

/// <summary>
/// A 2D plot, e.g. a graph of a function.
///
/// <see cref="Plot"/> supports multiple lines and points.
/// </summary>
/// <remarks>
/// This only covers a starter subset of `egui_plot::Plot`'s builder options. Several of the
/// upstream options (label/axis formatters, custom grid spacers, per-axis configuration) are
/// backed by closures on the Rust side, which can't be serialized across the FFI boundary and so
/// aren't exposed here.
/// </remarks>
public struct Plot
{
    private Id _id;
    private float? _viewAspect;
    private float? _width;
    private float? _height;
    private bool _showX;
    private bool _showY;
    private EVec2b _showGrid;
    private EVec2b _allowZoom;
    private EVec2b _allowDrag;
    private Legend? _legend;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("'Plot' does not contain a constructor that takes 0 arguments", error: true)]
    public Plot() { throw new InvalidOperationException(); }

    /// <summary>
    /// Give a unique id for each plot within the same <see cref="Ui"/>.
    /// </summary>
    public Plot(Id id)
    {
        _id = id;
        _viewAspect = null;
        _width = null;
        _height = null;
        _showX = true;
        _showY = true;
        _showGrid = new EVec2b { X = true, Y = true };
        _allowZoom = new EVec2b { X = true, Y = true };
        _allowDrag = new EVec2b { X = true, Y = true };
        _legend = null;
    }

    /// <summary>
    /// Width / height ratio of the plot region. By default no fixed aspect ratio is set (and
    /// width/height will fill the ui it is in).
    /// </summary>
    public readonly Plot ViewAspect(float viewAspect)
    {
        var result = this;
        result._viewAspect = viewAspect;
        return result;
    }

    /// <summary>
    /// Width of plot. By default a plot will fill the ui it is in. If you set <see cref="ViewAspect"/>,
    /// the width can be calculated from the height.
    /// </summary>
    public readonly Plot Width(float width)
    {
        var result = this;
        result._width = width;
        return result;
    }

    /// <summary>
    /// Height of plot. By default a plot will fill the ui it is in. If you set <see cref="ViewAspect"/>,
    /// the height can be calculated from the width.
    /// </summary>
    public readonly Plot Height(float height)
    {
        var result = this;
        result._height = height;
        return result;
    }

    /// <summary>
    /// Show the x-value (e.g. when hovering). Default: <c>true</c>.
    /// </summary>
    public readonly Plot ShowX(bool showX)
    {
        var result = this;
        result._showX = showX;
        return result;
    }

    /// <summary>
    /// Show the y-value (e.g. when hovering). Default: <c>true</c>.
    /// </summary>
    public readonly Plot ShowY(bool showY)
    {
        var result = this;
        result._showY = showY;
        return result;
    }

    /// <summary>
    /// Show a grid overlay on the plot. Default: <c>true</c>.
    /// </summary>
    public readonly Plot ShowGrid(EVec2b show)
    {
        var result = this;
        result._showGrid = show;
        return result;
    }

    /// <summary>
    /// Whether to allow zooming in the plot. Default: <c>true</c>.
    /// </summary>
    public readonly Plot AllowZoom(EVec2b allow)
    {
        var result = this;
        result._allowZoom = allow;
        return result;
    }

    /// <summary>
    /// Whether to allow dragging in the plot to move the bounds. Default: <c>true</c>.
    /// </summary>
    public readonly Plot AllowDrag(EVec2b allow)
    {
        var result = this;
        result._allowDrag = allow;
        return result;
    }

    /// <summary>
    /// Show a legend including all named items.
    /// </summary>
    public readonly Plot Legend(Legend legend)
    {
        var result = this;
        result._legend = legend;
        return result;
    }

    /// <summary>
    /// Interact with and add items to the plot, then draw it.
    /// </summary>
    public readonly PlotResponse Show(Ui ui, Action<PlotUi> buildFn)
    {
        using var callback = new EguiCallback(ptr => buildFn(new PlotUi(ptr)));
        var (response, transform, hoveredPlotItem) = EguiMarshal.Call<nuint, PlotInner, EguiCallback, (Response, PlotTransform, Id?)>(
            EguiFn.egui_plot_plot_Plot_show, ui.Ptr, new PlotInner(this), callback);

        return new PlotResponse
        {
            Response = response,
            Transform = transform,
            HoveredPlotItem = hoveredPlotItem
        };
    }

    /// <inheritdoc cref="Show"/>
    public readonly PlotResponse<R> Show<R>(Ui ui, Func<PlotUi, R> buildFn)
    {
        R result = default!;
        using var callback = new EguiCallback(ptr => result = buildFn(new PlotUi(ptr)));
        var (response, transform, hoveredPlotItem) = EguiMarshal.Call<nuint, PlotInner, EguiCallback, (Response, PlotTransform, Id?)>(
            EguiFn.egui_plot_plot_Plot_show, ui.Ptr, new PlotInner(this), callback);

        return new PlotResponse<R>
        {
            Inner = result,
            Response = response,
            Transform = transform,
            HoveredPlotItem = hoveredPlotItem
        };
    }

    /// <summary>
    /// Helper struct used for serialization.
    /// </summary>
    private struct PlotInner
    {
        private Id _id;
        private float? _viewAspect;
        private float? _width;
        private float? _height;
        private bool _showX;
        private bool _showY;
        private EVec2b _showGrid;
        private EVec2b _allowZoom;
        private EVec2b _allowDrag;
        private Legend? _legend;

        public PlotInner(Plot plot)
        {
            _id = plot._id;
            _viewAspect = plot._viewAspect;
            _width = plot._width;
            _height = plot._height;
            _showX = plot._showX;
            _showY = plot._showY;
            _showGrid = plot._showGrid;
            _allowZoom = plot._allowZoom;
            _allowDrag = plot._allowDrag;
            _legend = plot._legend;
        }

        internal static void Serialize(BincodeSerializer serializer, PlotInner value) => value.Serialize(serializer);

        internal readonly void Serialize(BincodeSerializer serializer)
        {
            serializer.increase_container_depth();
            _id.Serialize(serializer);
            TraitHelpers.serialize_option_f32(_viewAspect, serializer);
            TraitHelpers.serialize_option_f32(_width, serializer);
            TraitHelpers.serialize_option_f32(_height, serializer);
            serializer.serialize_bool(_showX);
            serializer.serialize_bool(_showY);
            _showGrid.Serialize(serializer);
            _allowZoom.Serialize(serializer);
            _allowDrag.Serialize(serializer);
            TraitHelpers.serialize_option_Legend(_legend, serializer);
            serializer.decrease_container_depth();
        }

        internal static PlotInner Deserialize(BincodeDeserializer deserializer)
        {
            deserializer.increase_container_depth();
            PlotInner obj = default;
            obj._id = Id.Deserialize(deserializer);
            obj._viewAspect = TraitHelpers.deserialize_option_f32(deserializer);
            obj._width = TraitHelpers.deserialize_option_f32(deserializer);
            obj._height = TraitHelpers.deserialize_option_f32(deserializer);
            obj._showX = deserializer.deserialize_bool();
            obj._showY = deserializer.deserialize_bool();
            obj._showGrid = EVec2b.Deserialize(deserializer);
            obj._allowZoom = EVec2b.Deserialize(deserializer);
            obj._allowDrag = EVec2b.Deserialize(deserializer);
            obj._legend = TraitHelpers.deserialize_option_Legend(deserializer);
            deserializer.decrease_container_depth();
            return obj;
        }
    }
}
