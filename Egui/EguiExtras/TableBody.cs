using System.Collections.Immutable;

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
    /// Add a single row with the given height.<br/>
    ///
    /// It is much more performant to use <see cref="Rows"/> or <see cref="HeterogeneousRows"/>,
    /// as those functions will only render the visible rows.
    /// </summary>
    public void Row(float height, Action<TableRow> addRowContent)
    {
        var ctx = _ctx;
        using var callback = new EguiCallback(rowPtr => addRowContent(new TableRow(ctx, rowPtr)));
        EguiMarshal.Call(EguiFn.egui_extras_table_TableBody_row, Ptr, height, callback);
    }

    /// <summary>
    /// Add many rows with the same height.<br/>
    ///
    /// Is a lot more performant than adding each individual row via <see cref="Row"/>, as
    /// non-visible rows are not rendered.<br/>
    ///
    /// If you need many rows with different heights, use <see cref="HeterogeneousRows"/>
    /// instead.<br/>
    ///
    /// This consumes the <see cref="TableBody"/>: no further rows may be added afterward.
    /// </summary>
    public void Rows(float rowHeightSansSpacing, nuint totalRows, Action<TableRow> addRowContent)
    {
        var ctx = _ctx;
        using var callback = new EguiCallback(rowPtr => addRowContent(new TableRow(ctx, rowPtr)));
        EguiMarshal.Call(EguiFn.egui_extras_table_TableBody_rows, Ptr, rowHeightSansSpacing, totalRows, callback);
    }

    /// <summary>
    /// Add rows with varying heights.<br/>
    ///
    /// This takes a very slight performance hit compared to <see cref="Rows"/> due to the need
    /// to iterate over all row heights in order to calculate the virtual table height above and
    /// below the visible region, but it is many orders of magnitude more performant than adding
    /// individual heterogeneously-sized rows using <see cref="Row"/>.<br/>
    ///
    /// This consumes the <see cref="TableBody"/>: no further rows may be added afterward.
    /// </summary>
    public void HeterogeneousRows(ImmutableArray<float> heights, Action<TableRow> addRowContent)
    {
        var ctx = _ctx;
        using var callback = new EguiCallback(rowPtr => addRowContent(new TableRow(ctx, rowPtr)));
        EguiMarshal.Call(EguiFn.egui_extras_table_TableBody_heterogeneous_rows, Ptr, heights, callback);
    }
}
