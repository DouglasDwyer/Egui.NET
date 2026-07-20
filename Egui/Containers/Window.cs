namespace Egui.Containers;

/// <summary>
/// Builder for a floating window which can be dragged, closed, collapsed, resized and scrolled (off by default).
///
/// You can customize:
/// <list type="bullet">
/// <item><description>title</description></item>
/// <item><description>default, minimum, maximum and/or fixed size, collapsed/expanded</description></item>
/// <item><description>if the window has a scroll area (off by default)</description></item>
/// <item><description>if the window can be collapsed (minimized) to just the title bar (yes, by default)</description></item>
/// <item><description>if there should be a close button (none by default)</description></item>
/// </list>
///
/// The previous rectangle used by this window can be obtained through <see cref="Memory.AreaRect"/>.
///
/// Note that this is NOT a native OS window.
/// To create a new native OS window, use <see cref="Context.ShowViewportDeferred"/> .
/// </summary>
public ref struct Window
{
    private Atoms _title;
    private ref bool _open;
    private bool _hasOpen;
    private Area _area;
    private Frame? _frame;
    private Resize _resize;
    private ScrollArea _scroll;
    private bool _collapsible;
    private bool _defaultOpen;
    private bool _withTitleBar;
    private bool _fadeOut;
    private bool _autoSized;
    private WindowDrag _dragArea;

    /// <summary>
    /// The window title is used as a unique Id and must be unique, and should not change.
    /// This is true even if you disable the title bar with <c>TitleBar(false)</c>.
    /// If you need a changing title, you must call <see cref="Id"/> with a fixed id.
    /// </summary>
    public Window(Atoms title)
    {
        Area area = new Area(title.Text ?? string.Empty).Kind(UiKind.Window);
        _title = title;
        _hasOpen = false;
        _area = area;
        _frame = null;
        _resize = new Resize()
            .WithStroke(false)
            .MinSize(new EVec2(96.0f, 32.0f))
            .DefaultSize(new EVec2(340.0f, 420.0f));
        _scroll = ScrollArea.Neither.AutoShrink(new EVec2b(false, false));
        _collapsible = true;
        _defaultOpen = true;
        _withTitleBar = true;
        _fadeOut = true;
        _autoSized = false;
        _dragArea = WindowDrag.OnTouch;
    }

    private Window(Window previous, ref bool open)
    {
        this = previous;
        _hasOpen = true;
        _open = ref open;
    }

    /// <summary>
    /// Returns <c>null</c> if the window is not open (if <see cref="Open"/>  was called with <c>false</c>).
    /// Returns <c>new InnerResponse { Inner = default })</c> if the window is collapsed.
    /// </summary>
    public readonly InnerResponse? Show(Context ctx, Action<Ui> addContents)
    {
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));

        bool? isOpen = _hasOpen ? _open : null;
        var (response, setOpen) = EguiMarshal.Call<nuint, WindowInner, bool?, EguiCallback, (Response?, bool)>(EguiFn.egui_containers_window_Window_show, ctx.Ptr, new WindowInner(this), isOpen, callback);

        if (_hasOpen)
        {
            _open = setOpen;
        }

        if (response.HasValue)
        {
            return new InnerResponse
            {
                Response = response.Value
            };
        }
        else
        {
            return null;
        }
    }

    /// <inheritdoc cref="Show"/>
    public readonly InnerResponse<R?>? Show<R>(Context ctx, Func<Ui, R> addContents)
    {
        R? result = default;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));

        bool? isOpen = _hasOpen ? _open : null;
        var (response, setOpen) = EguiMarshal.Call<nuint, WindowInner, bool?, EguiCallback, (Response?, bool)>(EguiFn.egui_containers_window_Window_show, ctx.Ptr, new WindowInner(this), isOpen, callback);

        if (_hasOpen)
        {
            _open = setOpen;
        }

        if (response.HasValue)
        {
            return new InnerResponse<R?>
            {
                Inner = result,
                Response = response.Value
            };
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Assign a unique id to the Window. Required if the title changes, or is shared with another window.
    /// </summary>
    public readonly Window Id(Id id)
    {
        var result = this;
        result._area = result._area.Id(id);
        return result;
    }

    /// <summary>
    /// Call this to add a close-button to the window title bar.
    ///
    /// * If open == false, the window will not be visible.
    /// * If open == true, the window will have a close button.
    /// * If the close button is pressed, open will be set to false.
    /// </summary>
    public readonly Window Open(ref bool open)
    {
        return new Window(this, ref open);
    }

    /// <summary>
    /// If false the window will be grayed out and non-interactive.
    /// </summary>
    public readonly Window Enabled(bool enabled)
    {
        var result = this;
        result._area = result._area.Enabled(enabled);
        return result;
    }

    /// <summary>
    /// If false, clicks go straight through to what is behind us.
    ///
    /// Can be used for semi-invisible areas that the user should be able to click through.
    ///
    /// Default: true.
    /// </summary>
    public readonly Window Interactable(bool interactable)
    {
        var result = this;
        result._area = result._area.Interactable(interactable);
        return result;
    }

    /// <summary>
    /// If false the window will be immovable.
    /// </summary>
    public readonly Window Movable(bool movable)
    {
        var result = this;
        result._area = result._area.Movable(movable);
        return result;
    }

    /// <summary>
    /// order(Order.Foreground) for a Window that should always be on top
    /// </summary>
    public readonly Window Order(Order order)
    {
        var result = this;
        result._area = result._area.Order(order);
        return result;
    }

    /// <summary>
    /// If true, quickly fade in the Window when it first appears.
    ///
    /// Default: true.
    /// </summary>
    public readonly Window FadeIn(bool fadeIn)
    {
        var result = this;
        result._area = result._area.FadeIn(fadeIn);
        return result;
    }

    /// <summary>
    /// If true, quickly fade out the Window when it closes.
    ///
    /// This only works if you use Self::open to close the window.
    ///
    /// Default: true.
    /// </summary>
    public readonly Window FadeOut(bool fadeOut)
    {
        var result = this;
        result._fadeOut = fadeOut;
        return result;
    }

    /// <summary>
    /// Usage: <c>new Window(...).Resize(r => r.AutoExpandWidth(true))</c>
    /// </summary>
    public readonly Window Resize(Func<Resize, Resize> mutate)
    {
        var result = this;
        result._resize = mutate(_resize);
        return result;
    }

    /// <summary>
    /// Change the background color, margins, etc.
    /// </summary>
    public readonly Window Frame(Frame frame)
    {
        var result = this;
        result._frame = frame;
        return result;
    }

    /// <summary>
    /// Set minimum width of the window.
    /// </summary>
    public readonly Window MinWidth(float minWidth)
    {
        var result = this;
        result._resize = result._resize.MinWidth(minWidth);
        return result;
    }

    /// <summary>
    /// Set minimum height of the window.
    /// </summary>
    public readonly Window MinHeight(float minHeight)
    {
        var result = this;
        result._resize = result._resize.MinHeight(minHeight);
        return result;
    }

    /// <summary>
    /// Set minimum size of the window, equivalent to calling both MinWidth and MinHeight.
    /// </summary>
    public readonly Window MinSize(EVec2 minSize)
    {
        var result = this;
        result._resize = result._resize.MinSize(minSize);
        return result;
    }

    /// <summary>
    /// Set maximum width of the window.
    /// </summary>
    public readonly Window MaxWidth(float maxWidth)
    {
        var result = this;
        result._resize = result._resize.MaxWidth(maxWidth);
        return result;
    }

    /// <summary>
    /// Set maximum height of the window.
    /// </summary>
    public readonly Window MaxHeight(float maxHeight)
    {
        var result = this;
        result._resize = result._resize.MaxHeight(maxHeight);
        return result;
    }

    /// <summary>
    /// Set maximum size of the window, equivalent to calling both MaxWidth and MaxHeight.
    /// </summary>
    public readonly Window MaxSize(EVec2 maxSize)
    {
        var result = this;
        result._resize = result._resize.MaxSize(maxSize);
        return result;
    }

    /// <summary>
    /// Set current position of the window.
    /// If the window is movable it is up to you to keep track of where it moved to!
    /// </summary>
    public readonly Window CurrentPos(EPos2 currentPos)
    {
        var result = this;
        result._area = result._area.CurrentPos(currentPos);
        return result;
    }

    /// <summary>
    /// Set initial position of the window.
    /// </summary>
    public readonly Window DefaultPos(EPos2 defaultPos)
    {
        var result = this;
        result._area = result._area.DefaultPos(defaultPos);
        return result;
    }

    /// <summary>
    /// Sets the window position and prevents it from being dragged around.
    /// </summary>
    public readonly Window FixedPos(EPos2 pos)
    {
        var result = this;
        result._area = result._area.FixedPos(pos);
        return result;
    }

    /// <summary>
    /// Constrains this window to Context.screen_rect.
    ///
    /// To change the area to constrain to, use Self::constrain_to.
    ///
    /// Default: true.
    /// </summary>
    public readonly Window Constrain(bool constrain)
    {
        var result = this;
        result._area = result._area.Constrain(constrain);
        return result;
    }

    /// <summary>
    /// Constrain the movement of the window to the given rectangle.
    ///
    /// For instance: .ConstrainTo(ctx.ScreenRect()).
    /// </summary>
    public readonly Window ConstrainTo(Rect constrainRect)
    {
        var result = this;
        result._area = result._area.ConstrainTo(constrainRect);
        return result;
    }

    /// <summary>
    /// Where the "root" of the window is.
    ///
    /// For instance, if you set this to Align2.RIGHT_TOP
    /// then Self::FixedPos will set the position of the right-top
    /// corner of the window.
    ///
    /// Default: Align2.LEFT_TOP.
    /// </summary>
    public readonly Window Pivot(Align2 pivot)
    {
        var result = this;
        result._area = result._area.Pivot(pivot);
        return result;
    }

    /// <summary>
    /// Set anchor and distance.
    ///
    /// An anchor of Align2.RIGHT_TOP means "put the right-top corner of the window
    /// in the right-top corner of the screen".
    ///
    /// The offset is added to the position, so e.g. an offset of [-5.0, 5.0]
    /// would move the window left and down from the given anchor.
    ///
    /// Anchoring also makes the window immovable.
    ///
    /// It is an error to set both an anchor and a position.
    /// </summary>
    public readonly Window Anchor(Align2 align, EVec2 offset)
    {
        var result = this;
        result._area = result._area.Anchor(align, offset);
        return result;
    }

    /// <summary>
    /// Set initial collapsed state of the window
    /// </summary>
    public readonly Window DefaultOpen(bool defaultOpen)
    {
        var result = this;
        result._defaultOpen = defaultOpen;
        return result;
    }

    /// <summary>
    /// Set initial size of the window.
    /// </summary>
    public readonly Window DefaultSize(EVec2 defaultSize)
    {
        var result = this;
        result._resize = result._resize.DefaultSize(defaultSize);
        result._area = result._area.DefaultSize(defaultSize);
        return result;
    }

    /// <summary>
    /// Set initial width of the window.
    /// </summary>
    public readonly Window DefaultWidth(float defaultWidth)
    {
        var result = this;
        result._resize = result._resize.DefaultWidth(defaultWidth);
        result._area = result._area.DefaultWidth(defaultWidth);
        return result;
    }

    /// <summary>
    /// Set initial height of the window.
    /// </summary>
    public readonly Window DefaultHeight(float defaultHeight)
    {
        var result = this;
        result._resize = result._resize.DefaultHeight(defaultHeight);
        result._area = result._area.DefaultHeight(defaultHeight);
        return result;
    }

    /// <summary>
    /// Sets the window size and prevents it from being resized by dragging its edges.
    /// </summary>
    public readonly Window FixedSize(EVec2 size)
    {
        var result = this;
        result._resize = result._resize.FixedSize(size);
        return result;
    }

    /// <summary>
    /// Set initial position and size of the window.
    /// </summary>
    public readonly Window DefaultRect(Rect rect)
    {
        var result = this;
        return this.DefaultPos(rect.Min).DefaultSize(rect.Size);
    }

    /// <summary>
    /// Sets the window pos and size and prevents it from being moved and resized by dragging its edges.
    /// </summary>
    public readonly Window FixedRect(Rect rect)
    {
        var result = this;
        return this.FixedPos(rect.Min).FixedSize(rect.Size);
    }

    /// <summary>
    /// Can the user resize the window by dragging its edges?
    /// </summary>
    public readonly Window Resizable(EVec2b resizable)
    {
        var result = this;
        result._resize = result._resize.Resizable(resizable);
        return result;
    }

    /// <summary>
    /// Can the window be collapsed by clicking on its title?
    /// </summary>
    public readonly Window Collapsible(bool collapsible)
    {
        var result = this;
        result._collapsible = collapsible;
        return result;
    }

    /// <summary>
    /// Show title bar on top of the window?
    /// </summary>
    public readonly Window TitleBar(bool titleBar)
    {
        var result = this;
        result._withTitleBar = titleBar;
        return result;
    }

    /// <summary>
    /// Not resizable, just takes the size of its contents.
    /// </summary>
    public readonly Window AutoSized()
    {
        var result = this;
        result._resize = result._resize.AutoSized();
        result._scroll = ScrollArea.Neither;
        result._autoSized = true;
        return result;
    }

    /// <summary>
    /// Sets where the user can drag to move the window. Defaults to <see cref="WindowDrag.OnTouch"/>.
    /// </summary>
    public readonly Window DragArea(WindowDrag dragArea)
    {
        var result = this;
        var inner = EguiMarshal.Call<WindowInner, WindowDrag, WindowInner>(EguiFn.egui_containers_window_Window_drag_area, new WindowInner(this), dragArea);
        inner.ApplyTo(ref result);
        return result;
    }

    /// <summary>
    /// Construct a <see cref="Window"/> that follows the given viewport.
    /// </summary>
    public static Window FromViewport(ViewportId id, ViewportBuilder viewport)
    {
        var inner = EguiMarshal.Call<ViewportId, ViewportBuilder, WindowInner>(EguiFn.egui_containers_window_Window_from_viewport, id, viewport);
        return inner.ToWindow();
    }

    /// <summary>
    /// Enable/disable horizontal/vertical scrolling. <c>false</c> by default.
    /// </summary>
    public readonly Window Scroll(EVec2b scroll)
    {
        var result = this;
        result._scroll = result._scroll.Scroll(scroll);
        return result;
    }

    /// <summary>
    /// Enable/disable horizontal scrolling. <c>false</c> by default.
    /// </summary>
    public readonly Window HScroll(bool hscroll)
    {
        var result = this;
        result._scroll = result._scroll.Hscroll(hscroll);
        return result;
    }

    /// <summary>
    /// Enable/disable vertical scrolling. <c>false</c> by default.
    /// </summary>
    public readonly Window VScroll(bool vscroll)
    {
        var result = this;
        result._scroll = result._scroll.Vscroll(vscroll);
        return result;
    }

    /// <summary>
    /// Enable/disable scrolling on the window by dragging with the pointer. <c>true</c> by default.
    /// </summary>
    public readonly Window DragToScroll(bool dragToScroll)
    {
        var result = this;
        result._scroll = result._scroll.ScrollSource(new ScrollSource
        {
            Drag = dragToScroll ? DragScroll.Always : DragScroll.Never,
            // Other properties can be set here as needed
        });
        return result;
    }

    /// <summary>
    /// Sets the <c>ScrollBarVisibility</c> of the window.
    /// </summary>
    public readonly Window ScrollBarVisibility(ScrollBarVisibility visibility)
    {
        var result = this;
        result._scroll = result._scroll.ScrollBarVisibility(visibility);
        return result;
    }

    /// <summary>
    /// Helper struct used for serialization.
    /// </summary>
    private struct WindowInner
    {
        private Egui.Atoms _title;
        private Egui.Containers.Area _area;
        private Egui.Containers.Frame? _frame;
        private Egui.Containers.Resize _resize;
        private Egui.Containers.ScrollArea _scroll;
        private bool _collapsible;
        private bool _defaultOpen;
        private bool _withTitleBar;
        private bool _fadeOut;
        private bool _autoSized;
        private Egui.Containers.WindowDrag _dragArea;

        public WindowInner(Window window)
        {
            _title = window._title;
            _area = window._area;
            _frame = window._frame;
            _resize = window._resize;
            _scroll = window._scroll;
            _collapsible = window._collapsible;
            _defaultOpen = window._defaultOpen;
            _withTitleBar = window._withTitleBar;
            _fadeOut = window._fadeOut;
            _autoSized = window._autoSized;
            _dragArea = window._dragArea;
        }

        /// <summary>
        /// Copies these fields onto an existing <see cref="Window"/> (everything but <c>Open</c>,
        /// which isn't part of the wire representation - see <see cref="ToWindow"/>).
        /// </summary>
        public readonly void ApplyTo(ref Window window)
        {
            window._title = _title;
            window._area = _area;
            window._frame = _frame;
            window._resize = _resize;
            window._scroll = _scroll;
            window._collapsible = _collapsible;
            window._defaultOpen = _defaultOpen;
            window._withTitleBar = _withTitleBar;
            window._fadeOut = _fadeOut;
            window._autoSized = _autoSized;
            window._dragArea = _dragArea;
        }

        /// <summary>
        /// Creates a fresh <see cref="Window"/> (with <c>Open</c> unset) from these fields.
        /// </summary>
        public readonly Window ToWindow()
        {
            Window window = default;
            window._hasOpen = false;
            ApplyTo(ref window);
            return window;
        }

        internal static void Serialize(BincodeSerializer serializer, WindowInner value) => value.Serialize(serializer);

        internal void Serialize(BincodeSerializer serializer)
        {
            serializer.increase_container_depth();
            _title.Serialize(serializer);
            _area.Serialize(serializer);
            Egui.TraitHelpers.serialize_option_Frame(_frame, serializer);
            _resize.Serialize(serializer);
            _scroll.Serialize(serializer);
            serializer.serialize_bool(_collapsible);
            serializer.serialize_bool(_defaultOpen);
            serializer.serialize_bool(_withTitleBar);
            serializer.serialize_bool(_fadeOut);
            serializer.serialize_bool(_autoSized);
            _dragArea.Serialize(serializer);
            serializer.decrease_container_depth();
        }

        internal static WindowInner Deserialize(BincodeDeserializer deserializer)
        {
            deserializer.increase_container_depth();
            WindowInner obj = default;
            obj._title = Egui.Atoms.Deserialize(deserializer);
            obj._area = Egui.Containers.Area.Deserialize(deserializer);
            obj._frame = Egui.TraitHelpers.deserialize_option_Frame(deserializer);
            obj._resize = Egui.Containers.Resize.Deserialize(deserializer);
            obj._scroll = Egui.Containers.ScrollArea.Deserialize(deserializer);
            obj._collapsible = deserializer.deserialize_bool();
            obj._defaultOpen = deserializer.deserialize_bool();
            obj._withTitleBar = deserializer.deserialize_bool();
            obj._fadeOut = deserializer.deserialize_bool();
            obj._autoSized = deserializer.deserialize_bool();
            obj._dragArea = Egui.Containers.WindowDragSerdeExtensions.Deserialize(deserializer);

            deserializer.decrease_container_depth();
            return obj;
        }
    }
}