namespace Egui.EguiExtras;

/// <summary>
/// A strip of cells which go in one direction, created with <see cref="StripBuilder"/>. Each
/// cell has a fixed size. In contrast to normal <see cref="Ui"/> behavior, strip cells do
/// <i>not</i> grow with their children.
/// </summary>
public readonly ref partial struct Strip
{
    internal readonly nuint Ptr;

    private readonly Context _ctx;

    internal Strip(Context ctx, nuint ptr)
    {
        _ctx = ctx;
        Ptr = ptr;
    }

    /// <summary>
    /// Add cell contents.
    /// </summary>
    public void Cell(Action<Ui> addContents)
    {
        var ctx = _ctx;
        using var callback = new EguiCallback(uiPtr => addContents(new Ui(ctx, uiPtr)));
        EguiMarshal.Call(EguiFn.egui_extras_strip_Strip_cell, Ptr, callback);
    }

    /// <summary>
    /// Add a nested strip as a cell.
    /// </summary>
    public void NestedStrip(Action<StripBuilder> addStripContents)
    {
        Cell(ui => addStripContents(new StripBuilder(ui)));
    }
}
