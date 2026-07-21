using Egui.Containers;

namespace Egui.EguiExtras;

/// <summary>
/// Builder for a table with (optional) fixed header and scrolling body.<br/>
/// You must pre-allocate all columns with <see cref="Column"/>.
/// </summary>
public struct TableBuilder
{
    private string? _idSalt;
    private List<Column> _columns;
    private bool? _striped;
    private Layout? _cellLayout;
    private bool _resizable;
    private Egui.Sense _sense;
    private bool _vscroll;
    private DragScroll _dragToScroll;
    private bool _stickToBottom;
    private (nuint Row, Align? Align)? _scrollToRow;
    private float? _verticalScrollOffset;
    private float _minScrolledHeight;
    private float _maxScrollHeight;
    private EVec2b _autoShrink;
    private Egui.Containers.ScrollBarVisibility _scrollBarVisibility;
    private bool _animateScrolling;

    public TableBuilder()
    {
        _columns = new List<Column>();
        _resizable = false;
        _sense = Egui.Sense.Hover;
        _vscroll = true;
        _dragToScroll = DragScroll.OnTouch;
        _stickToBottom = false;
        _minScrolledHeight = 200.0f;
        _maxScrollHeight = float.PositiveInfinity;
        _autoShrink = new EVec2b { X = true, Y = true };
        _scrollBarVisibility = Egui.Containers.ScrollBarVisibility.VisibleWhenNeeded;
        _animateScrolling = true;
    }

    /// <summary>
    /// Give this table a unique salt within the parent <see cref="Ui"/>.<br/>
    /// This is required if you have multiple tables in the same <see cref="Ui"/>.
    /// </summary>
    public readonly TableBuilder IdSalt(string idSalt)
    {
        var result = this;
        result._idSalt = idSalt;
        return result;
    }

    /// <summary>
    /// Allocate space for one column.
    /// </summary>
    public readonly TableBuilder Column(Column column)
    {
        var result = this;
        result._columns = new List<Column>(_columns) { column };
        return result;
    }

    /// <summary>
    /// Allocate space for several columns at once.
    /// </summary>
    public readonly TableBuilder Columns(Column column, int count)
    {
        var result = this;
        var newColumns = new List<Column>(_columns);
        for (int i = 0; i < count; i++)
        {
            newColumns.Add(column);
        }
        result._columns = newColumns;
        return result;
    }

    /// <summary>
    /// Enable striped row background for improved readability.<br/>
    /// Default is whatever is in the current <c>Visuals.Striped</c>.
    /// </summary>
    public readonly TableBuilder Striped(bool striped)
    {
        var result = this;
        result._striped = striped;
        return result;
    }

    /// <summary>
    /// What layout should be used for the individual cells?
    /// </summary>
    public readonly TableBuilder CellLayout(Layout cellLayout)
    {
        var result = this;
        result._cellLayout = cellLayout;
        return result;
    }

    /// <summary>
    /// Make the columns resizable by dragging. Default: <c>false</c>.
    /// </summary>
    public readonly TableBuilder Resizable(bool resizable)
    {
        var result = this;
        result._resizable = resizable;
        return result;
    }

    /// <summary>
    /// What should table cells sense for? (default: hover).
    /// </summary>
    public readonly TableBuilder Sense(Egui.Sense sense)
    {
        var result = this;
        result._sense = sense;
        return result;
    }

    /// <summary>
    /// Enable vertical scrolling in the body (default: <c>true</c>).
    /// </summary>
    public readonly TableBuilder VScroll(bool vscroll)
    {
        var result = this;
        result._vscroll = vscroll;
        return result;
    }

    /// <summary>
    /// Controls scrolling the table's contents by dragging with the pointer.<br/>
    /// Defaults to only being active when a touch screen is detected.
    /// </summary>
    public readonly TableBuilder DragToScroll(DragScroll dragToScroll)
    {
        var result = this;
        result._dragToScroll = dragToScroll;
        return result;
    }

    /// <summary>
    /// Should the scroll handle stick to the bottom position even as the content size changes
    /// dynamically? Default: <c>false</c>.
    /// </summary>
    public readonly TableBuilder StickToBottom(bool stick)
    {
        var result = this;
        result._stickToBottom = stick;
        return result;
    }

    /// <summary>
    /// Set a row to scroll to, positioned according to <paramref name="align"/> (or just enough to bring
    /// it into view if <paramref name="align"/> is <c>null</c>).
    /// </summary>
    public readonly TableBuilder ScrollToRow(nuint row, Align? align = null)
    {
        var result = this;
        result._scrollToRow = (row, align);
        return result;
    }

    /// <summary>
    /// Set the vertical scroll offset position, in points.
    /// </summary>
    public readonly TableBuilder VerticalScrollOffset(float offset)
    {
        var result = this;
        result._verticalScrollOffset = offset;
        return result;
    }

    /// <summary>
    /// The minimum height of a vertical scroll area which requires scroll bars. Default: <c>200.0</c>.
    /// </summary>
    public readonly TableBuilder MinScrolledHeight(float minScrolledHeight)
    {
        var result = this;
        result._minScrolledHeight = minScrolledHeight;
        return result;
    }

    /// <summary>
    /// Don't make the scroll area higher than this (add scroll bars instead). Default: <c>800.0</c>.
    /// </summary>
    public readonly TableBuilder MaxScrollHeight(float maxScrollHeight)
    {
        var result = this;
        result._maxScrollHeight = maxScrollHeight;
        return result;
    }

    /// <summary>
    /// For each axis: if <c>true</c>, add blank space outside the table, keeping the table small;
    /// if <c>false</c>, add blank space inside the table, expanding it to fit the containing <see cref="Ui"/>.
    /// Default: <c>true</c> for both axes.
    /// </summary>
    public readonly TableBuilder AutoShrink(EVec2b autoShrink)
    {
        var result = this;
        result._autoShrink = autoShrink;
        return result;
    }

    /// <summary>
    /// Set the visibility of both horizontal and vertical scroll bars.
    /// </summary>
    public readonly TableBuilder ScrollBarVisibility(Egui.Containers.ScrollBarVisibility scrollBarVisibility)
    {
        var result = this;
        result._scrollBarVisibility = scrollBarVisibility;
        return result;
    }

    /// <summary>
    /// Should the scroll area animate scroll-to functions? Default: <c>true</c>.
    /// </summary>
    public readonly TableBuilder AnimateScrolling(bool animate)
    {
        var result = this;
        result._animateScrolling = animate;
        return result;
    }

    /// <summary>
    /// Create the table body, without a header row.
    /// </summary>
    public readonly void Show(Ui ui, Action<TableBody> body) => ShowInternal(ui, null, null, body);

    /// <summary>
    /// Create a header row which always stays visible and at the top, followed by the table body.
    /// </summary>
    public readonly void Show(Ui ui, float headerHeight, Action<TableRow> header, Action<TableBody> body) => ShowInternal(ui, headerHeight, header, body);

    private readonly void ShowInternal(Ui ui, float? headerHeight, Action<TableRow>? header, Action<TableBody> body)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;

        EguiCallback? headerCallback = header is not null
            ? new EguiCallback(rowPtr => header(new TableRow(ctx, rowPtr)))
            : null;

        try
        {
            using var bodyCallback = new EguiCallback(bodyPtr => body(new TableBody(ctx, bodyPtr)));
            EguiMarshal.Call<nuint, TableBuilder, float?, EguiCallback?, EguiCallback, (Id, State, EVec2, Rect)>(
                EguiFn.egui_extras_table_TableBuilder_show, ui.Ptr, this, headerHeight, headerCallback, bodyCallback);
        }
        finally
        {
            if (headerCallback is EguiCallback hc)
            {
                ((IDisposable)hc).Dispose();
            }
        }
    }

    internal static void Serialize(Bincode.BincodeSerializer serializer, TableBuilder value) => value.Serialize(serializer);

    internal readonly void Serialize(Bincode.BincodeSerializer serializer)
    {
        serializer.increase_container_depth();

        Egui.TraitHelpers.serialize_option_str(_idSalt, serializer);

        serializer.serialize_len(_columns.Count);
        foreach (var column in _columns)
        {
            column.Serialize(serializer);
        }

        Egui.TraitHelpers.serialize_option_bool(_striped, serializer);

        if (_cellLayout is Layout cellLayout)
        {
            serializer.serialize_option_tag(true);
            cellLayout.Serialize(serializer);
        }
        else
        {
            serializer.serialize_option_tag(false);
        }

        serializer.serialize_bool(_resizable);
        _sense.Serialize(serializer);
        serializer.serialize_bool(_vscroll);
        _dragToScroll.Serialize(serializer);
        serializer.serialize_bool(_stickToBottom);

        if (_scrollToRow is (nuint row, var align))
        {
            serializer.serialize_option_tag(true);
            serializer.serialize_u64(row);
            if (align is Align a)
            {
                serializer.serialize_option_tag(true);
                a.Serialize(serializer);
            }
            else
            {
                serializer.serialize_option_tag(false);
            }
        }
        else
        {
            serializer.serialize_option_tag(false);
        }

        Egui.TraitHelpers.serialize_option_f32(_verticalScrollOffset, serializer);
        serializer.serialize_f32(_minScrolledHeight);
        serializer.serialize_f32(_maxScrollHeight);
        _autoShrink.Serialize(serializer);
        _scrollBarVisibility.Serialize(serializer);
        serializer.serialize_bool(_animateScrolling);

        serializer.decrease_container_depth();
    }

    internal static TableBuilder Deserialize(Bincode.BincodeDeserializer deserializer) => throw new NotImplementedException();
}
