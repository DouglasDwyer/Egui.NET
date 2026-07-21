namespace Egui.EguiExtras;

/// <summary>
/// The body of a table. Do not store this: it should be used immediately.
/// </summary>
public readonly ref struct TableBody
{
    internal readonly nuint Ptr;

    private readonly Context _ctx;

    internal TableBody(Context ctx, nuint ptr)
    {
        _ctx = ctx;
        Ptr = ptr;
    }

    /// <summary>
    /// Add a single row with the given height.
    /// </summary>
    public void Row(float height, Action<TableRow> addRowContent)
    {
        var ctx = _ctx;
        using var callback = new EguiCallback(rowPtr => addRowContent(new TableRow(ctx, rowPtr)));
        EguiMarshal.Call(EguiFn.egui_extras_table_TableBody_row, Ptr, height, callback);
    }
}
