namespace Egui.EguiPlot;

/// <summary>
/// Provides methods to interact with a plot while building it. It is the single argument of the
/// closure provided to <see cref="Plot.Show{R}"/>.
/// </summary>
public ref partial struct PlotUi
{
    /// <summary>
    /// A pointer to the underlying <c>egui_plot::PlotUi</c> object.
    /// </summary>
    internal readonly nuint Ptr;

    /// <summary>
    /// Creates a new <see cref="PlotUi"/> that references the given pointer.
    /// </summary>
    /// <param name="ptr">The native object pointer.</param>
    internal PlotUi(nuint ptr)
    {
        Ptr = ptr;
    }

    /// <summary>
    /// Throws an exception if this is a null object.
    /// </summary>
    internal readonly void AssertInitialized()
    {
        if (Ptr == 0) { throw new NullReferenceException("PlotUi instance was uninitialized"); }
    }

    /// <summary>
    /// Add a data line.
    /// </summary>
    public readonly void Line(Line line)
    {
        AssertInitialized();
        EguiMarshal.Call(EguiFn.egui_plot_plot_PlotUi_line, Ptr, line);
    }

    /// <summary>
    /// Add data points.
    /// </summary>
    public readonly void Points(Points points)
    {
        AssertInitialized();
        EguiMarshal.Call(EguiFn.egui_plot_plot_PlotUi_points, Ptr, points);
    }
}
