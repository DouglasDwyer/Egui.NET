namespace Egui.EguiPlot;

/// <summary>
/// What <see cref="Plot.Show"/> returns.
/// </summary>
public struct PlotResponse
{
    /// <summary>
    /// The response of the plot.
    /// </summary>
    public required Response Response;

    /// <summary>
    /// The transform between screen coordinates and plot coordinates.
    /// </summary>
    public required PlotTransform Transform;

    /// <summary>
    /// The id of a currently hovered item, if any.
    ///
    /// This is <c>null</c> if no item was hovered. A plot item can be hovered either by hovering
    /// its representation in the plot (line, marker, etc.) or by hovering the item in the legend.
    /// </summary>
    public Id? HoveredPlotItem;
}

/// <summary>
/// What <see cref="Plot.Show{R}"/> returns.
/// </summary>
/// <typeparam name="R">The type of the closure's return value.</typeparam>
public struct PlotResponse<R>
{
    /// <summary>
    /// What the user closure returned.
    /// </summary>
    public required R Inner;

    /// <summary>
    /// The response of the plot.
    /// </summary>
    public required Response Response;

    /// <summary>
    /// The transform between screen coordinates and plot coordinates.
    /// </summary>
    public required PlotTransform Transform;

    /// <summary>
    /// The id of a currently hovered item, if any.
    ///
    /// This is <c>null</c> if no item was hovered. A plot item can be hovered either by hovering
    /// its representation in the plot (line, marker, etc.) or by hovering the item in the legend.
    /// </summary>
    public Id? HoveredPlotItem;
}
