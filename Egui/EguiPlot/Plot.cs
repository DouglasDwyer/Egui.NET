namespace Egui.EguiPlot;

public partial record struct Plot
{
    /// <summary>
    /// Interact with and add items to the plot, then draw it.
    /// </summary>
    public readonly PlotResponse Show(Ui ui, Action<PlotUi> buildFn)
    {
        using var callback = new EguiCallback(ptr => buildFn(new PlotUi(ptr)));
        var (response, transform, hoveredPlotItem) = EguiMarshal.Call<nuint, Plot, EguiCallback, (Response, PlotTransform, Id?)>(
            EguiFn.egui_plot_plot_Plot_show, ui.Ptr, this, callback);

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
        var (response, transform, hoveredPlotItem) = EguiMarshal.Call<nuint, Plot, EguiCallback, (Response, PlotTransform, Id?)>(
            EguiFn.egui_plot_plot_Plot_show, ui.Ptr, this, callback);

        return new PlotResponse<R>
        {
            Inner = result,
            Response = response,
            Transform = transform,
            HoveredPlotItem = hoveredPlotItem
        };
    }
}
