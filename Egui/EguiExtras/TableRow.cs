namespace Egui.EguiExtras;

/// <summary>
/// The row of a table. Do not store this: it should be used immediately.
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
    /// Add the contents of a column.
    /// </summary>
    public (Rect, Response) Col(Action<Ui> addContents)
    {
        var ctx = _ctx;
        using var callback = new EguiCallback(uiPtr => addContents(new Ui(ctx, uiPtr)));
        return EguiMarshal.Call<nuint, EguiCallback, (Rect, Response)>(EguiFn.egui_extras_table_TableRow_col, Ptr, callback);
    }

    /// <summary>
    /// Set the selected visual state of this row.
    /// </summary>
    public void SetSelected(bool selected) => EguiMarshal.Call(EguiFn.egui_extras_table_TableRow_set_selected, Ptr, selected);

    /// <summary>
    /// Set the hovered visual state of this row.
    /// </summary>
    public void SetHovered(bool hovered) => EguiMarshal.Call(EguiFn.egui_extras_table_TableRow_set_hovered, Ptr, hovered);

    /// <summary>
    /// Draw a subtle line above this row.
    /// </summary>
    public void SetOverline(bool overline) => EguiMarshal.Call(EguiFn.egui_extras_table_TableRow_set_overline, Ptr, overline);

    /// <summary>
    /// Returns the response of the whole row.
    /// </summary>
    public Response Response() => EguiMarshal.Call<nuint, Response>(EguiFn.egui_extras_table_TableRow_response, Ptr);
}
