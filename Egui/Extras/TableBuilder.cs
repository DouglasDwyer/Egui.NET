using System.ComponentModel;
using Egui.Containers;

namespace Egui.Extras;

/// <summary>
/// Builder for a <see cref="Table"/> with (optional) fixed header and scrolling body.<br/>
/// You must pre-allocate all columns with <see cref="Column"/>/<see cref="Columns"/>.<br/>
///
/// If you have multiple tables in the same <see cref="Ui"/> you will need to give them unique
/// ids with <see cref="IdSalt"/>.
/// </summary>
public ref struct TableBuilder
{
    private Ui _ui;
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

    /// <summary>
    /// The <see cref="Ui"/> that this table is being built within.
    /// </summary>
    internal readonly Ui Ui => _ui;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("'TableBuilder' does not contain a constructor that takes 0 arguments", error: true)]
    public TableBuilder() { throw new InvalidOperationException(); }

    public TableBuilder(Ui ui)
    {
        _ui = ui;
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
    /// Enable striped row background for improved readability.<br/>
    /// Default is whatever is in <see cref="Visuals.Striped"/>.
    /// </summary>
    public readonly TableBuilder Striped(bool striped)
    {
        var result = this;
        result._striped = striped;
        return result;
    }

    /// <summary>
    /// What should table cells sense for? (default: <see cref="Egui.Sense.Hover"/>).
    /// </summary>
    public readonly TableBuilder Sense(Egui.Sense sense)
    {
        var result = this;
        result._sense = sense;
        return result;
    }

    /// <summary>
    /// Make the columns resizable by dragging.<br/>
    ///
    /// You can set this for individual columns with <see cref="Extras.Column.Resizable"/>.
    /// <see cref="Resizable"/> is used as a fallback for any column for which you don't call
    /// <see cref="Extras.Column.Resizable"/>.<br/>
    ///
    /// If the _last_ column is <see cref="Extras.Column.Remainder"/>, then it won't be
    /// resizable (and instead use up the remainder).<br/>
    ///
    /// Default is <c>false</c>.
    /// </summary>
    public readonly TableBuilder Resizable(bool resizable)
    {
        var result = this;
        result._resizable = resizable;
        return result;
    }

    /// <summary>
    /// Enable vertical scrolling in body (default: <c>true</c>)
    /// </summary>
    public readonly TableBuilder VScroll(bool vscroll)
    {
        var result = this;
        result._vscroll = vscroll;
        return result;
    }

    /// <summary>
    /// Controls scrolling the table's contents by dragging with the pointer.<br/>
    ///
    /// Defaults to <see cref="DragScroll.OnTouch"/> &#8212; only active when a touch screen is
    /// detected.
    /// </summary>
    public readonly TableBuilder DragToScroll(DragScroll dragToScroll)
    {
        var result = this;
        result._dragToScroll = dragToScroll;
        return result;
    }

    /// <summary>
    /// Should the scroll handle stick to the bottom position even as the content size changes
    /// dynamically? The scroll handle remains stuck until manually changed, and will become
    /// stuck once again when repositioned to the bottom. Default: <c>false</c>.
    /// </summary>
    public readonly TableBuilder StickToBottom(bool stick)
    {
        var result = this;
        result._stickToBottom = stick;
        return result;
    }

    /// <summary>
    /// Set a row to scroll to.<br/>
    ///
    /// <paramref name="align"/> specifies if the row should be positioned in the top, center, or
    /// bottom of the view. If <paramref name="align"/> is <c>null</c>, the table will scroll just
    /// enough to bring the cursor into view.<br/>
    ///
    /// See also: <see cref="VerticalScrollOffset"/>.
    /// </summary>
    public readonly TableBuilder ScrollToRow(nuint row, Align? align = null)
    {
        var result = this;
        result._scrollToRow = (row, align);
        return result;
    }

    /// <summary>
    /// Set the vertical scroll offset position, in points.<br/>
    ///
    /// See also: <see cref="ScrollToRow"/>.
    /// </summary>
    public readonly TableBuilder VerticalScrollOffset(float offset)
    {
        var result = this;
        result._verticalScrollOffset = offset;
        return result;
    }

    /// <summary>
    /// The minimum height of a vertical scroll area which requires scroll bars.<br/>
    ///
    /// The scroll area will only become smaller than this if the content is smaller than this
    /// (and so we don't require scroll bars).<br/>
    ///
    /// Default: <c>200.0</c>.
    /// </summary>
    public readonly TableBuilder MinScrolledHeight(float minScrolledHeight)
    {
        var result = this;
        result._minScrolledHeight = minScrolledHeight;
        return result;
    }

    /// <summary>
    /// Don't make the scroll area higher than this (add scroll-bars instead!).<br/>
    ///
    /// In other words: add scroll-bars when this height is reached.
    /// Default: <c>800.0</c>.
    /// </summary>
    public readonly TableBuilder MaxScrollHeight(float maxScrollHeight)
    {
        var result = this;
        result._maxScrollHeight = maxScrollHeight;
        return result;
    }

    /// <summary>
    /// For each axis (x,y):
    /// <list type="bullet">
    /// <item>If <c>true</c>, add blank space outside the table, keeping the table small.</item>
    /// <item>If <c>false</c>, add blank space inside the table, expanding the table to fit the
    /// containing <see cref="Ui"/>.</item>
    /// </list>
    ///
    /// Default: <c>true</c>.
    /// </summary>
    public readonly TableBuilder AutoShrink(EVec2b autoShrink)
    {
        var result = this;
        result._autoShrink = autoShrink;
        return result;
    }

    /// <summary>
    /// Set the visibility of both horizontal and vertical scroll bars.<br/>
    ///
    /// With <see cref="Egui.Containers.ScrollBarVisibility.VisibleWhenNeeded"/> (default), the
    /// scroll bar will be visible only when needed.
    /// </summary>
    public readonly TableBuilder ScrollBarVisibility(Egui.Containers.ScrollBarVisibility scrollBarVisibility)
    {
        var result = this;
        result._scrollBarVisibility = scrollBarVisibility;
        return result;
    }

    /// <summary>
    /// Should the scroll area animate <c>ScrollTo*</c> functions?<br/>
    ///
    /// Default: <c>true</c>.
    /// </summary>
    public readonly TableBuilder AnimateScrolling(bool animated)
    {
        var result = this;
        result._animateScrolling = animated;
        return result;
    }

    /// <summary>
    /// What layout should we use for the individual cells?
    /// </summary>
    public readonly TableBuilder CellLayout(Layout cellLayout)
    {
        var result = this;
        result._cellLayout = cellLayout;
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
    /// Create a header row which always stays visible and at the top
    /// </summary>
    public readonly Table Header(float height, Action<TableRow> addHeaderRow)
    {
        return new Table(this, height, addHeaderRow);
    }

    /// <summary>
    /// Create table body without a header row
    /// </summary>
    public readonly ScrollAreaOutput Body(Action<TableBody> addBodyContents)
    {
        return Show(null, null, addBodyContents);
    }

    internal readonly ScrollAreaOutput Show(float? headerHeight, Action<TableRow>? headerCallback, Action<TableBody> addBodyContents)
    {
        _ui.AssertInitialized();
        var ctx = _ui.Ctx;
        var options = ToOptions();

        EguiCallback? headerEguiCallback = headerCallback is not null
            ? new EguiCallback(rowPtr => headerCallback(new TableRow(ctx, rowPtr)))
            : null;

        try
        {
            using var bodyEguiCallback = new EguiCallback(bodyPtr => addBodyContents(new TableBody(ctx, bodyPtr)));
            var (id, state, contentSize, innerRect) = EguiMarshal.Call<nuint, TableOptions, float?, EguiCallback?, EguiCallback, (Id, State, EVec2, Rect)>(
                EguiFn.egui_extras_table_TableBuilder_show, _ui.Ptr, options, headerHeight, headerEguiCallback, bodyEguiCallback);
            return new ScrollAreaOutput
            {
                Id = id,
                State = state,
                ContentSize = contentSize,
                InnerRect = innerRect
            };
        }
        finally
        {
            if (headerEguiCallback is EguiCallback hc)
            {
                ((IDisposable)hc).Dispose();
            }
        }
    }

    private readonly TableOptions ToOptions() => new TableOptions
    {
        IdSalt = _idSalt,
        Columns = _columns,
        Striped = _striped,
        CellLayout = _cellLayout,
        Resizable = _resizable,
        Sense = _sense,
        VScroll = _vscroll,
        DragToScroll = _dragToScroll,
        StickToBottom = _stickToBottom,
        ScrollToRow = _scrollToRow,
        VerticalScrollOffset = _verticalScrollOffset,
        MinScrolledHeight = _minScrolledHeight,
        MaxScrollHeight = _maxScrollHeight,
        AutoShrink = _autoShrink,
        ScrollBarVisibility = _scrollBarVisibility,
        AnimateScrolling = _animateScrolling
    };

    /// <summary>
    /// The wire representation of a <see cref="TableBuilder"/>'s options, sent alongside the
    /// <see cref="Ui"/> pointer and callbacks when the table is shown.
    /// </summary>
    private struct TableOptions
    {
        public string? IdSalt;
        public List<Column> Columns;
        public bool? Striped;
        public Layout? CellLayout;
        public bool Resizable;
        public Egui.Sense Sense;
        public bool VScroll;
        public DragScroll DragToScroll;
        public bool StickToBottom;
        public (nuint Row, Align? Align)? ScrollToRow;
        public float? VerticalScrollOffset;
        public float MinScrolledHeight;
        public float MaxScrollHeight;
        public EVec2b AutoShrink;
        public Egui.Containers.ScrollBarVisibility ScrollBarVisibility;
        public bool AnimateScrolling;

        internal static void Serialize(Bincode.BincodeSerializer serializer, TableOptions value) => value.Serialize(serializer);

        internal readonly void Serialize(Bincode.BincodeSerializer serializer)
        {
            serializer.increase_container_depth();

            Egui.TraitHelpers.serialize_option_str(IdSalt, serializer);

            serializer.serialize_len(Columns.Count);
            foreach (var column in Columns)
            {
                column.Serialize(serializer);
            }

            Egui.TraitHelpers.serialize_option_bool(Striped, serializer);

            if (CellLayout is Layout cellLayout)
            {
                serializer.serialize_option_tag(true);
                cellLayout.Serialize(serializer);
            }
            else
            {
                serializer.serialize_option_tag(false);
            }

            serializer.serialize_bool(Resizable);
            Sense.Serialize(serializer);
            serializer.serialize_bool(VScroll);
            DragToScroll.Serialize(serializer);
            serializer.serialize_bool(StickToBottom);

            if (ScrollToRow is (nuint row, var align))
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

            Egui.TraitHelpers.serialize_option_f32(VerticalScrollOffset, serializer);
            serializer.serialize_f32(MinScrolledHeight);
            serializer.serialize_f32(MaxScrollHeight);
            AutoShrink.Serialize(serializer);
            ScrollBarVisibility.Serialize(serializer);
            serializer.serialize_bool(AnimateScrolling);

            serializer.decrease_container_depth();
        }

        internal static TableOptions Deserialize(Bincode.BincodeDeserializer deserializer) => throw new NotImplementedException();
    }
}
