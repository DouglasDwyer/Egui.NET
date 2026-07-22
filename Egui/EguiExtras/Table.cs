namespace Egui.EguiExtras;

/// <summary>
/// A table, created after adding a header row via <see cref="TableBuilder.Header"/>.
/// </summary>
public readonly ref struct Table
{
    private readonly TableBuilder _builder;
    private readonly float _headerHeight;
    private readonly Action<TableRow> _headerCallback;

    internal Table(TableBuilder builder, float headerHeight, Action<TableRow> headerCallback)
    {
        _builder = builder;
        _headerHeight = headerHeight;
        _headerCallback = headerCallback;
    }

    /// <summary>
    /// Create table body after adding a header row
    /// </summary>
    public readonly void Body(Action<TableBody> addBodyContents)
    {
        _builder.Show(_headerHeight, _headerCallback, addBodyContents);
    }
}
