namespace Egui.Plot;

public partial record struct Plot
{
    /// <summary>
    /// Whether to allow zooming in the plot. Default: <c>true</c>.<br/>
    ///
    /// Note: Allowing zoom in one axis but not the other may lead to unexpected
    /// results if used in combination with <see cref="DataAspect"/>.
    /// </summary>
    public readonly Plot AllowZoom(EVec2b on)
    {
        return EguiMarshal.Call<Plot, EVec2b, Plot>(EguiFn.egui_plot_plot_Plot_allow_zoom, this, on);
    }

    /// <summary>
    /// Whether to allow scrolling in the plot. Default: <c>true</c>.
    /// </summary>
    public readonly Plot AllowScroll(EVec2b on)
    {
        return EguiMarshal.Call<Plot, EVec2b, Plot>(EguiFn.egui_plot_plot_Plot_allow_scroll, this, on);
    }

    /// <summary>
    /// Whether to allow dragging in the plot to move the bounds. Default: <c>true</c>.<br/>
    ///
    /// The button to use is specified by <see cref="PanPointerButton"/>.
    /// </summary>
    public readonly Plot AllowDrag(EVec2b on)
    {
        return EguiMarshal.Call<Plot, EVec2b, Plot>(EguiFn.egui_plot_plot_Plot_allow_drag, this, on);
    }

    /// <summary>
    /// Whether to allow dragging in the axis areas to zoom the plot. Default: <c>true</c>.
    /// </summary>
    public readonly Plot AllowAxisZoomDrag(EVec2b on)
    {
        return EguiMarshal.Call<Plot, EVec2b, Plot>(EguiFn.egui_plot_plot_Plot_allow_axis_zoom_drag, this, on);
    }

    /// <summary>
    /// Interact with and add items to the plot, then draw it.
    /// </summary>
    public readonly PlotResponse Show(Ui ui, Action<PlotUi> buildFn)
    {
        var ctx = ui.Ctx;
        using var callback = new EguiCallback.Pin(ptr => buildFn(new PlotUi(ctx, ptr)));
        var (response, transform, hoveredPlotItem) = EguiMarshal.Call<nuint, Plot, EguiCallback, (Response, PlotTransform, Id?)>(
            EguiFn.egui_plot_plot_Plot_show, ui.Ptr, this, callback.AsNative());

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
        var ctx = ui.Ctx;
        R result = default!;
        using var callback = new EguiCallback.Pin(ptr => result = buildFn(new PlotUi(ctx, ptr)));
        var (response, transform, hoveredPlotItem) = EguiMarshal.Call<nuint, Plot, EguiCallback, (Response, PlotTransform, Id?)>(
            EguiFn.egui_plot_plot_Plot_show, ui.Ptr, this, callback.AsNative());

        return new PlotResponse<R>
        {
            Inner = result,
            Response = response,
            Transform = transform,
            HoveredPlotItem = hoveredPlotItem
        };
    }
}
