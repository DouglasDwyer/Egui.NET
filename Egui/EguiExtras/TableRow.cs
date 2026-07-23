namespace Egui.EguiExtras;

/// <summary>
/// The row of a table.
/// </summary>
public readonly ref partial struct TableRow
{
    internal readonly nuint Ptr;

    private readonly Context _ctx;

    internal TableRow(Context ctx, nuint ptr)
    {
        _ctx = ctx;
        Ptr = ptr;
    }

    /// <summary>
    /// Add the contents of a column on this row (i.e. a cell).<br/>
    ///
    /// Returns the used space (<c>MinRect</c>) plus the <see cref="Response"/> of the whole cell.
    /// </summary>
    public (Rect, Response) Col(Action<Ui> addCellContents)
    {
        var ctx = _ctx;
        using var callback = new EguiCallback(uiPtr => addCellContents(new Ui(ctx, uiPtr)));
        return EguiMarshal.Call<nuint, EguiCallback, (Rect, Response)>(EguiFn.egui_extras_table_TableRow_col, Ptr, callback);
    }

    /// <summary>
    /// The index of the row.
    /// </summary>
    public nuint Index => EguiMarshal.Call<nuint, nuint>(EguiFn.egui_extras_table_TableRow_index, Ptr);
}
