using System.ComponentModel;
using Egui.Containers;

namespace Egui.Extras;

/// <summary>
/// A table, created after adding a header row via <see cref="TableBuilder.Header"/>.
/// </summary>
public readonly ref struct Table
{
    private readonly TableBuilder _builder;
    private readonly float _headerHeight;
    private readonly Action<TableRow> _headerCallback;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("'Table' does not contain a constructor that takes 0 arguments", error: true)]
    public Table() { throw new InvalidOperationException(); }

    internal Table(TableBuilder builder, float headerHeight, Action<TableRow> headerCallback)
    {
        _builder = builder;
        _headerHeight = headerHeight;
        _headerCallback = headerCallback;
    }

    /// <summary>
    /// The <see cref="Ui"/> that this table is being drawn within. Can be used to add
    /// extra widgets between the header and the body.
    /// </summary>
    public readonly Ui Ui => _builder.Ui;

    /// <summary>
    /// Create table body after adding a header row
    /// </summary>
    public readonly ScrollAreaOutput Body(Action<TableBody> addBodyContents)
    {
        return _builder.Show(_headerHeight, _headerCallback, addBodyContents);
    }
}
