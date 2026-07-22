namespace Egui.EguiExtras;

/// <summary>
/// The row of a table.
/// </summary>
public readonly ref struct TableRow
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
    /// Set the selection highlight state for cells added after a call to this function.
    /// </summary>
    public void SetSelected(bool selected) => EguiMarshal.Call(EguiFn.egui_extras_table_TableRow_set_selected, Ptr, selected);

    /// <summary>
    /// Set the hovered highlight state for cells added after a call to this function.
    /// </summary>
    public void SetHovered(bool hovered) => EguiMarshal.Call(EguiFn.egui_extras_table_TableRow_set_hovered, Ptr, hovered);

    /// <summary>
    /// Set the overline state for this row. The overline is a line above the row,
    /// usable for e.g. visually grouping rows.
    /// </summary>
    public void SetOverline(bool overline) => EguiMarshal.Call(EguiFn.egui_extras_table_TableRow_set_overline, Ptr, overline);

    /// <summary>
    /// Returns a union of the <see cref="Response"/>s of the cells added to the row up to this point.<br/>
    ///
    /// You need to add at least one row to the table before reading this property.
    /// </summary>
    public Response Response => EguiMarshal.Call<nuint, Response>(EguiFn.egui_extras_table_TableRow_response, Ptr);
}
