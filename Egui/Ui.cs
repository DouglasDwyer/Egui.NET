using Egui.Util;

namespace Egui;

/// <summary>
/// This is what you use to place widgets.
/// Represents a region of the screen with a type of layout (horizontal or vertical).
/// </summary>
public readonly ref partial struct Ui
{
    /// <summary>
    /// Use this to paint stuff within this <see cref="Ui"/> .
    /// </summary>
    public Painter Painter
    {
        get
        {
            AssertInitialized();
            return new Painter(Ctx, EguiMarshal.Call<nuint, EguiHandle>(EguiFn.egui_ui_Ui_painter, Ptr));
        }
    }

    /// <summary>
    /// Read the <see cref="Layout"/> 
    /// </summary>
    public Layout Layout
    {
        get
        {
            AssertInitialized();
            return EguiMarshal.Call<nuint, Layout>(EguiFn.egui_ui_Ui_layout, Ptr);
        }
    }

    /// <summary>
    /// The current spacing options for this <see cref="Ui"/> . Short for <c>ui.Style.Spacing</c>.
    /// </summary>
    public Spacing Spacing
    {
        get
        {
            AssertInitialized();
            return EguiMarshal.Call<nuint, Spacing>(EguiFn.egui_ui_Ui_spacing, Ptr);
        }
        set
        {
            AssertInitialized();
            EguiMarshal.Call(EguiFn.egui_ui_Ui_spacing_mut, Ptr, value);
        }
    }

    /// <summary>
    /// Get a reference to this <see cref="Ui"/>'s <see cref="UiStack"/> .
    /// </summary>
    public UiStack Stack
    {
        get
        {
            AssertInitialized();
            return EguiMarshal.Call<nuint, UiStack>(EguiFn.egui_ui_Ui_stack, Ptr);
        }
    }

    /// <summary>
    /// Style options for this <see cref="Ui"/>  and its children.
    /// Note that this may be a different <see cref="Style"/>  than that of <see cref="Context.Style"/> .
    /// </summary>
    public Style Style
    {
        get
        {
            AssertInitialized();
            return EguiMarshal.Call<nuint, Style>(EguiFn.egui_ui_Ui_style, Ptr);
        }
        set
        {
            AssertInitialized();
            EguiMarshal.Call(EguiFn.egui_ui_Ui_style_mut, Ptr, value);
        }
    }

    /// <summary>
    /// The current visuals settings of this <see cref="Ui"/> . Short for <c>ui.Style.Visuals</c>.
    /// </summary>
    public Visuals Visuals
    {
        get
        {
            AssertInitialized();
            return EguiMarshal.Call<nuint, Visuals>(EguiFn.egui_ui_Ui_visuals, Ptr);
        }
        set
        {
            AssertInitialized();
            EguiMarshal.Call(EguiFn.egui_ui_Ui_visuals_mut, Ptr, value);
        }
    }

    /// <summary>
    /// Get a reference to the parent <see cref="Context"/>. 
    /// </summary>
    public readonly Context Ctx;

    /// <summary>
    /// A pointer to the underlying UI object.
    /// </summary>
    internal readonly nuint Ptr;

    /// <summary>
    /// Creates a UI reference.
    /// </summary>
    /// <param name="ctx">The context that owns the UI.</param>
    /// <param name="ptr">A pointer to the UI.</param>
    internal Ui(Context ctx, nuint ptr)
    {
        Ctx = ctx;
        Ptr = ptr;
    }

    /// <inheritdoc cref="Context.Fonts"/>
    public void Fonts(Action<Fonts> reader) => Ctx.Fonts(reader);

    /// <inheritdoc cref="Context.Fonts"/>
    public R Fonts<R>(Func<Fonts, R> reader) => Ctx.Fonts(reader);

    /// <inheritdoc cref="Context.Input"/>
    public void Input(Action<InputState> reader) => Ctx.Input(reader);

    /// <inheritdoc cref="Context.Input"/>
    public R Input<R>(Func<InputState, R> reader) => Ctx.Input(reader);

    /// <inheritdoc cref="Context.InputMut"/>
    public void InputMut(MutateDelegate<InputState> writer) => Ctx.InputMut(writer);

    /// <inheritdoc cref="Context.InputMut"/>
    public R InputMut<R>(MutateDelegate<InputState, R> writer) => Ctx.InputMut(writer);

    /// <inheritdoc cref="Context.Output"/>
    public void Output(Action<PlatformOutput> reader) => Ctx.Output(reader);

    /// <inheritdoc cref="Context.Output"/>
    public R Output<R>(Func<PlatformOutput, R> reader) => Ctx.Output(reader);

    /// <inheritdoc cref="Context.OutputMut"/>
    public void OutputMut(MutateDelegate<PlatformOutput> writer) => Ctx.OutputMut(writer);

    /// <inheritdoc cref="Context.OutputMut"/>
    public R OutputMut<R>(MutateDelegate<PlatformOutput, R> writer) => Ctx.OutputMut(writer);

    /// <inheritdoc cref="Context.MemoryMut"/>
    public void MemoryMut(Action<Memory> writer) => Ctx.MemoryMut(writer);

    /// <inheritdoc cref="Context.MemoryMut"/>
    public R MemoryMut<R>(Func<Memory, R> writer) => Ctx.MemoryMut(writer);

    /// <inheritdoc cref="Context.DataMut"/>
    public void DataMut(Action<IdTypeMap> writer) => Ctx.DataMut(writer);

    /// <inheritdoc cref="Context.DataMut"/>
    public R DataMut<R>(Func<IdTypeMap, R> writer) => Ctx.DataMut(writer);

    /// <summary>
    /// Add a <see cref="Widget"/>  to this <see cref="Ui"/>  at a location dependent on the current <see cref="Layout"/> .
    ///
    /// The returned <see cref="Response"/> can be used to check for interactions,
    /// as well as adding tooltips using <see cref="Response.OnHoverText"/> .
    ///
    /// See also <see cref="AddSized"/>  and <see cref="Put"/> .
    /// </summary>
    public readonly Response Add<T>(T widget) where T : IWidget, allows ref struct
    {
        AssertInitialized();
        return widget.Ui(this);
    }

    /// <summary>
    /// Add a single <see cref="IWidget"/> that is possibly disabled, i.e. greyed out and non-interactive.
    ///
    /// If you call <see cref="AddEnabled"/>  from within an already disabled <see cref="Ui"/> ,
    /// the widget will always be disabled, even if the <paramref name="enabled"/> argument is true.
    ///
    /// See also <see cref="AddEnabledUi"/>  and <see cref="IsEnabled"/> .
    /// </summary>
    public unsafe readonly Response AddEnabled<T>(bool enabled, T widget) where T : IWidget, allows ref struct
    {
#pragma warning disable CS8500
        if (IsEnabled && !enabled)
        {
            var tp = &widget;
            return Scope(ui =>
            {
                ui.Disable();
                return ui.Add(*tp);
            }).Inner;
        }
        else
        {
            return Add(widget);
        }         
#pragma warning restore CS8500
    }

    /// <summary>
    /// Add a <see cref="IWidget"/>  to this <see cref="Ui"/>  at a location dependent on the current <see cref="Layout"/> .
    ///
    /// The returned <see cref="Response"/> can be used to check for interactions,
    /// as well as adding tooltips using <see cref="Response.OnHoverText"/> .
    ///
    /// See also <see cref="AddSized"/>  and <see cref="Put"/> .
    /// </summary>
    public unsafe readonly Response AddSized<T>(EVec2 maxSize, T widget) where T : IWidget, allows ref struct
    {
#pragma warning disable CS8500
        var tp = &widget;
        var layout = Layout.CenteredAndJustified(Layout.MainDir);
        return AllocateUiWithLayout(maxSize, layout, ui => ui.Add(*tp)).Inner;
#pragma warning restore CS8500
    }

    /// <summary>
    /// Add a <see cref="IWidget"/>  to this <see cref="Ui"/> at a specific location (manual layout) without
    /// affecting this <see cref="Ui"/>s cursor.
    ///
    /// See also <see cref="Add"/>  and <see cref="AddSized"/>  and <see cref="Put"/> .
    /// </summary>
    public unsafe readonly Response Place<T>(Rect maxRect, T widget) where T : IWidget, allows ref struct
    {
#pragma warning disable CS8500
        AssertInitialized();
        var tp = &widget;
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => (*tp).Ui(new Ui(ctx, ui)));
        return EguiMarshal.Call<nuint, Rect, EguiCallback, Response>(EguiFn.egui_ui_Ui_place, Ptr, maxRect, callback);
#pragma warning restore CS8500
    }

    /// <summary>
    /// Add a Widget to this <see cref="Ui"/> at a specific location (manual layout).<br/>
    /// See also <see cref="Add"/>  and <see cref="AddSized"/>.
    /// </summary>
    public unsafe readonly Response Put<T>(Rect maxRect, T widget) where T : IWidget, allows ref struct
    {
#pragma warning disable CS8500
        var tp = &widget;
        return ScopeBuilder(
            new UiBuilder()
                .WithMaxRect(maxRect)
                .WithLayout(Layout.CenteredAndJustified(Direction.TopDown)),
            ui => ui.Add(*tp)
        ).Inner;
#pragma warning restore CS8500
    }

    /// <summary>
    /// Add a single <see cref="IWidget"/>  that is possibly invisible.
    ///
    /// An invisible widget still takes up the same space as if it were visible.
    ///
    /// If you call <see cref="AddVisible"/>  from within an already invisible <see cref="Ui"/> ,
    /// the widget will always be invisible, even if the <paramref name="visible"/> argument is true.
    ///
    /// See also <see cref="AddVisibleUi"/> , <see cref="SetVisible"/> and <see cref="IsVisible"/>.
    /// </summary>
    public unsafe readonly Response AddVisible<T>(bool visible, T widget) where T : IWidget, allows ref struct
    {
#pragma warning disable CS8500
        if (IsVisible && !visible)
        {
            var tp = &widget;
            return Scope(ui =>
            {
                ui.SetInvisible();
                return ui.Add(*tp);
            }).Inner;
        }
        else
        {
            return Add(widget);
        }
#pragma warning restore CS8500
    }

    /// <summary>
    /// Allocated the given space and then adds content to that space.
    /// If the contents overflow, more space will be allocated.
    /// When finished, the amount of space actually used (<c>min_rect</c>) will be allocated.
    /// So you can request a lot of space and then use less.
    /// </summary>
    public readonly InnerResponse AllocateUi(EVec2 desiredSize, Action<Ui> addContents)
    {
        return AllocateUiWithLayout(desiredSize, Layout, addContents);
    }

    /// <inheritdoc cref="AllocateUi"/>
    public readonly InnerResponse<R> AllocateUi<R>(EVec2 desiredSize, Func<Ui, R> addContents)
    {
        return AllocateUiWithLayout(desiredSize, Layout, addContents);
    }

    /// <summary>
    /// Allocated the given space and then adds content to that space.
    /// If the contents overflow, more space will be allocated.
    /// When finished, the amount of space actually used (<c>min_rect</c>) will be allocated.
    /// So you can request a lot of space and then use less.
    /// </summary>
    public readonly InnerResponse AllocateUiWithLayout(EVec2 desiredSize, Layout layout, Action<Ui> addContents)
    {
        AssertInitialized();
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, EVec2, Layout, EguiCallback, Response>(EguiFn.egui_ui_Ui_allocate_ui_with_layout, Ptr, desiredSize, layout, callback);
        return new InnerResponse
        {
            Response = response
        };
    }

    /// <inheritdoc cref="AllocateUiWithLayout"/>
    public readonly InnerResponse<R> AllocateUiWithLayout<R>(EVec2 desiredSize, Layout layout, Func<Ui, R> addContents)
    {
        AssertInitialized();
        R result = default!;
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, EVec2, Layout, EguiCallback, Response>(EguiFn.egui_ui_Ui_allocate_ui_with_layout, Ptr, desiredSize, layout, callback);
        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }

    /// <summary>
    /// Temporarily split a <see cref="Ui"/>  into several columns.
    /// </summary>
    public unsafe readonly void Columns(nuint numColumns, Action<UiArray> addContents)
    {
        AssertInitialized();
        var ctx = Ctx;
        using var callback = new EguiCallback(uiArray => addContents(new UiArray(ctx, (nuint*)uiArray, (int)numColumns)));
        EguiMarshal.Call(EguiFn.egui_ui_Ui_columns, Ptr, numColumns, callback);
    }

    /// <inheritdoc cref="Columns"/>
    public unsafe readonly R Columns<R>(nuint numColumns, Func<UiArray, R> addContents)
    {
        AssertInitialized();
        R result = default!;
        var ctx = Ctx;
        using var callback = new EguiCallback(uiArray => result = addContents(new UiArray(ctx, (nuint*)uiArray, (int)numColumns)));
        EguiMarshal.Call(EguiFn.egui_ui_Ui_columns, Ptr, numColumns, callback);
        return result;
    }

    /// <summary>
    /// Put into a <see cref="Frame.Group"/> , visually grouping the contents together
    /// </summary>
    public readonly InnerResponse Group(Action<Ui> addContents)
    {
        AssertInitialized();
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, EguiCallback, Response>(EguiFn.egui_ui_Ui_group, Ptr, callback);
        return new InnerResponse
        {
            Response = response
        };
    }

    /// <inheritdoc cref="Group"/>
    public readonly InnerResponse<R> Group<R>(Func<Ui, R> addContents)       
    {
        AssertInitialized();
        R result = default!;
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui))); 
        var response = EguiMarshal.Call<nuint, EguiCallback, Response>(EguiFn.egui_ui_Ui_group, Ptr, callback);
        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }

    /// <summary>
    /// This will make the next added widget centered and justified in the available space.<br/>
    ///
    /// Only one widget may be added to the inner <c>Ui</c>!
    /// </summary>
    public readonly InnerResponse CenteredAndJustified(Action<Ui> addContents)
    {
        AssertInitialized();
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, EguiCallback, Response>(EguiFn.egui_ui_Ui_centered_and_justified, Ptr, callback);
        return new InnerResponse
        {
            Response = response
        };
    }

    /// <inheritdoc cref="CenteredAndJustified"/>
    public readonly InnerResponse<R> CenteredAndJustified<R>(Func<Ui, R> addContents)       
    {
        AssertInitialized();
        R result = default!;
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui))); 
        var response = EguiMarshal.Call<nuint, EguiCallback, Response>(EguiFn.egui_ui_Ui_centered_and_justified, Ptr, callback);
        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }

    /// <summary>
    /// Start a ui with horizontal layout.
    /// After you have called this, the function registers the contents as any other widget.<br/>
    ///
    /// Elements will be centered on the Y axis, i.e.
    /// adjusted up and down to lie in the center of the horizontal layout.
    /// The initial height is <c>Style.spacing.interactSize.y</c>.
    /// Centering is almost always what you want if you are
    /// planning to mix widgets or use different types of text.<br/>
    ///
    /// If you don't want the contents to be centered, use <c>HorizontalTop</c> instead.<br/>
    ///
    /// The returned <c>Response</c> will only have checked for mouse hover
    /// but can be used for tooltips (<c>OnHoverText</c>).
    /// It also contains the <c>Rect</c> used by the horizontal layout.See also <c>WithLayout</c> for more options.
    /// </summary>
    public readonly InnerResponse Horizontal(Action<Ui> addContents)
    {
        AssertInitialized();
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, EguiCallback, Response>(EguiFn.egui_ui_Ui_horizontal, Ptr, callback);
        return new InnerResponse
        {
            Response = response
        };
    }

    /// <inheritdoc cref="Horizontal"/>
    public readonly InnerResponse<R> Horizontal<R>(Func<Ui, R> addContents)
    {
        AssertInitialized();
        R result = default!;
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, EguiCallback, Response>(EguiFn.egui_ui_Ui_horizontal, Ptr, callback);
        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }

    /// <summary>
    /// Like <c>Horizontal</c>, but allocates the full vertical height and then centers elements vertically.
    /// </summary>
    public readonly InnerResponse HorizontalCentered(Action<Ui> addContents)
    {
        AssertInitialized();
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, EguiCallback, Response>(EguiFn.egui_ui_Ui_horizontal_centered, Ptr, callback);
        return new InnerResponse
        {
            Response = response
        };
    }

    /// <inheritdoc cref="HorizontalCentered"/>
    public readonly InnerResponse<R> HorizontalCentered<R>(Func<Ui, R> addContents)
    {
        AssertInitialized();
        R result = default!;
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, EguiCallback, Response>(EguiFn.egui_ui_Ui_horizontal_centered, Ptr, callback);
        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }

    /// <summary>
    /// Like <c>Horizontal</c>, but aligns content with top.
    /// </summary>
    public readonly InnerResponse HorizontalTop(Action<Ui> addContents)
    {
        AssertInitialized();
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, EguiCallback, Response>(EguiFn.egui_ui_Ui_horizontal_top, Ptr, callback);
        return new InnerResponse
        {
            Response = response
        };
    }

    /// <inheritdoc cref="HorizontalTop"/>
    public readonly InnerResponse<R> HorizontalTop<R>(Func<Ui, R> addContents)
    {
        AssertInitialized();
        R result = default!;
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, EguiCallback, Response>(EguiFn.egui_ui_Ui_horizontal_top, Ptr, callback);
        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }

    /// <summary>
    /// Start a ui with horizontal layout that wraps to a new row
    /// when it reaches the right edge of the <c>MaxSize</c>.
    /// After you have called this, the function registers the contents as any other widget.<br/>
    ///
    /// Elements will be centered on the Y axis, i.e.
    /// adjusted up and down to lie in the center of the horizontal layout.
    /// The initial height is <c>Style.spacing.interactSize.y</c>.
    /// Centering is almost always what you want if you are
    /// planning to mix widgets or use different types of text.<br/>
    ///
    /// The returned <c>Response</c> will only have checked for mouse hover
    /// but can be used for tooltips (<c>OnHoverText</c>).
    /// It also contains the <c>Rect</c> used by the horizontal layout.<br/>
    ///
    /// See also <c>WithLayout</c> for more options.
    /// </summary>
    public readonly InnerResponse HorizontalWrapped(Action<Ui> addContents)
    {
        AssertInitialized();
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, EguiCallback, Response>(EguiFn.egui_ui_Ui_horizontal_wrapped, Ptr, callback);
        return new InnerResponse
        {
            Response = response
        };
    }

    /// <inheritdoc cref="HorizontalWrapped"/>
    public readonly InnerResponse<R> HorizontalWrapped<R>(Func<Ui, R> addContents)
    {
        AssertInitialized();
        R result = default!;
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, EguiCallback, Response>(EguiFn.egui_ui_Ui_horizontal_wrapped, Ptr, callback);
        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }

    /// <summary>
    /// Start a ui with vertical layout.
    /// Widgets will be left-justified.See also <c>WithLayout</c> for more options.
    /// </summary>
    public readonly InnerResponse Vertical(Action<Ui> addContents)
    {
        AssertInitialized();
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, EguiCallback, Response>(EguiFn.egui_ui_Ui_vertical, Ptr, callback);
        return new InnerResponse
        {
            Response = response
        };
    }

    /// <inheritdoc cref="Vertical"/>
    public readonly InnerResponse<R> Vertical<R>(Func<Ui, R> addContents)
    {
        AssertInitialized();
        R result = default!;
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, EguiCallback, Response>(EguiFn.egui_ui_Ui_vertical, Ptr, callback);
        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }

    /// <summary>
    /// Start a ui with vertical layout.
    /// Widgets will be horizontally centered.
    /// </summary>
    public readonly InnerResponse VerticalCentered(Action<Ui> addContents)
    {
        AssertInitialized();
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, EguiCallback, Response>(EguiFn.egui_ui_Ui_vertical_centered, Ptr, callback);
        return new InnerResponse
        {
            Response = response
        };
    }

    /// <inheritdoc cref="VerticalCentered"/>
    public readonly InnerResponse<R> VerticalCentered<R>(Func<Ui, R> addContents)
    {
        AssertInitialized();
        R result = default!;
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, EguiCallback, Response>(EguiFn.egui_ui_Ui_vertical_centered, Ptr, callback);
        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }

    /// <summary>
    /// Start a ui with vertical layout.
    /// Widgets will be horizontally centered and justified (fill full width).
    /// </summary>
    public readonly InnerResponse VerticalCenteredJustified(Action<Ui> addContents)
    {
        AssertInitialized();
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, EguiCallback, Response>(EguiFn.egui_ui_Ui_vertical_centered_justified, Ptr, callback);
        return new InnerResponse
        {
            Response = response
        };
    }

    /// <inheritdoc cref="VerticalCenteredJustified"/>
    public readonly InnerResponse<R> VerticalCenteredJustified<R>(Func<Ui, R> addContents)
    {
        AssertInitialized();
        R result = default!;
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, EguiCallback, Response>(EguiFn.egui_ui_Ui_vertical_centered_justified, Ptr, callback);
        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }

    /// <summary>
    /// Add a section that is possibly disabled, i.e. greyed out and non-interactive.
    /// </summary>
    public readonly InnerResponse AddEnabledUi(bool enabled, Action<Ui> addContents)
    {
        return Scope(ui =>
        {
            if (!enabled)
            {
                ui.Disable();
            }
            addContents(ui);
        });
    }

    /// <inheritdoc cref="AddEnabledUi"/>
    public readonly InnerResponse<R> AddEnabledUi<R>(bool enabled, Func<Ui, R> addContents)
    {
        return Scope(ui =>
        {
            if (!enabled)
            {
                ui.Disable();
            }
            return addContents(ui);
        });
    }

    /// <summary>
    /// A <see cref="CollapsingHeader"/> that starts out collapsed.<br/>
    /// The name must be unique within the current parent, or you need to use <see cref="CollapsingHeader.IdSalt"/>.
    /// </summary>
    public readonly CollapsingResponse Collapsing(WidgetText heading, Action<Ui> addContents)
    {
        return new CollapsingHeader(heading).Show(this, addContents);
    }

    /// <inheritdoc cref="Collapsing"/>
    public readonly CollapsingResponse<R> Collapsing<R>(WidgetText heading, Func<Ui, R> addContents)
    {
        return new CollapsingHeader(heading).Show(this, addContents);
    }

    /// <summary>
    /// Create a scoped child ui.
    /// </summary>
    public readonly InnerResponse Scope(Action<Ui> addContents)
    {
        return ScopeBuilder(new UiBuilder(), addContents);
    }

    /// <inheritdoc cref="Scope"/>
    public readonly InnerResponse<R> Scope<R>(Func<Ui, R> addContents)
    {
        return ScopeBuilder(new UiBuilder(), addContents);
    }
    
    /// <summary>
    /// This is the <see cref="Id"/> that will be assigned to the next widget added to this <c>Ui</c>.
    /// </summary>
    public readonly Id NextAutoId() {
        return EguiMarshal.Call<nuint, Id>(EguiFn.egui_ui_Ui_next_auto_id, Ptr);
    }

    /// <summary>
    /// Create a child Ui with an explicit <see cref="Id"/>.
    /// </summary>
    public readonly InnerResponse PushId(Id idSalt, Action<Ui> addContents)
    {
        return ScopeBuilder(new UiBuilder().WithIdSalt(idSalt), addContents);
    }

    /// <inheritdoc cref="PushId"/>
    public readonly InnerResponse<R> PushId<R>(Id idSalt, Func<Ui, R> addContents)
    {
        return ScopeBuilder(new UiBuilder().WithIdSalt(idSalt), addContents);
    }

    /// <summary>
    /// The new layout will take up all available space.<br/>
    /// If you don't want to use up all available space, use <see cref="AllocateUiWithLayout"/> .<br/>
    ///
    /// See also the helpers <see cref="Horizontal"/>, <see cref="Vertical"/>, etc.
    /// </summary>
    public readonly InnerResponse WithLayout(Layout layout, Action<Ui> addContents)
    {
        return ScopeBuilder(new UiBuilder().WithLayout(layout), addContents);
    }

    /// <inheritdoc cref="PushId"/>
    public readonly InnerResponse<R> WithLayout<R>(Layout layout, Func<Ui, R> addContents)
    {
        return ScopeBuilder(new UiBuilder().WithLayout(layout), addContents);
    }

    /// <summary>
    /// Create a child, add content to it, and then allocate only what was used in the parent <see cref="Ui"/>.
    /// </summary>
    public readonly InnerResponse ScopeBuilder(UiBuilder uiBuilder, Action<Ui> addContents)
    {
        AssertInitialized();
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, UiBuilder, EguiCallback, Response>(EguiFn.egui_ui_Ui_scope_builder, Ptr, uiBuilder, callback);
        return new InnerResponse
        {
            Response = response
        };
    }

    /// <inheritdoc cref="ScopeBuilder"/>
    public readonly InnerResponse<R> ScopeBuilder<R>(UiBuilder uiBuilder, Func<Ui, R> addContents)
    {
        AssertInitialized();
        R result = default!;
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, UiBuilder, EguiCallback, Response>(EguiFn.egui_ui_Ui_scope_builder, Ptr, uiBuilder, callback);
        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }

    /// <summary>
    /// Create a new Scope and transform its contents via a <see cref="TSTransform"/> .<br/>
    /// This only affects visuals, inputs will not be transformed. So this is mostly useful
    /// to create visual effects on interactions, e.g. scaling a button on hover / click.<br/>
    ///
    /// Check out <see cref="Context.SetTransformLayer"/> for a persistent transform that also affects
    /// inputs.
    /// </summary>
    public readonly InnerResponse WithVisualTransform(TSTransform transform, Action<Ui> addContents)
    {
        AssertInitialized();
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, TSTransform, EguiCallback, Response>(EguiFn.egui_ui_Ui_with_visual_transform, Ptr, transform, callback);
        return new InnerResponse
        {
            Response = response
        };
    }

    /// <inheritdoc cref="WithVisualTransform"/>
    public readonly InnerResponse<R> WithVisualTransform<R>(TSTransform transform, Func<Ui, R> addContents)
    {
        AssertInitialized();
        R result = default!;
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, TSTransform, EguiCallback, Response>(EguiFn.egui_ui_Ui_with_visual_transform, Ptr, transform, callback);
        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }

    /// <summary>
    /// Create a child ui which is indented to the right.<br/>
    ///
    /// The <paramref name="idSalt"/> here be anything at all.
    /// </summary>
    public readonly InnerResponse Indent(Id idSalt, Action<Ui> addContents)
    {
        AssertInitialized();
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, Id, EguiCallback, Response>(EguiFn.egui_ui_Ui_indent, Ptr, idSalt, callback);
        return new InnerResponse
        {
            Response = response
        };
    }

    /// <inheritdoc cref="Indent"/>
    public readonly InnerResponse<R> Indent<R>(Id idSalt, Func<Ui, R> addContents)
    {
        AssertInitialized();
        R result = default!;
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, Id, EguiCallback, Response>(EguiFn.egui_ui_Ui_indent, Ptr, idSalt, callback);
        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }

    /// <summary>
    /// Convenience function to get a region to paint on.
    /// Note that egui uses screen coordinates for everything.
    /// </summary>
    public readonly (Response, Painter) AllocatePainter(EVec2 desiredSize, Sense sense)
    {
        AssertInitialized();
        var (response, handle) = EguiMarshal.Call<nuint, EVec2, Sense, (Response, EguiHandle)>(EguiFn.egui_ui_Ui_painter_at, Ptr, desiredSize, sense);
        return (response, new Painter(Ctx, handle));
    }

    /// <summary>
    /// Create a painter for a sub-region of this <see cref="Ui"/>.
    /// The clip-rect of the returned <see cref="Painter"/>  will be the intersection of the given rectangle and the <see cref="ClipRect"/> of this <see cref="Ui"/>.
    /// </summary>
    public readonly Painter PainterAt(Rect rect)
    {
        AssertInitialized();
        return new Painter(Ctx, EguiMarshal.Call<nuint, Rect, EguiHandle>(EguiFn.egui_ui_Ui_painter_at, Ptr, rect));
    }

    /// <summary>
    /// Shortcut for <c>Default()</c><br/>
    /// 
    /// See also <see cref="Widgets.Separator"/> .
    /// </summary>
    public readonly Response Separator()
    {
        AssertInitialized();
        return EguiMarshal.Call<nuint, Response>(EguiFn.egui_ui_Ui_separator, Ptr);
    }

    /// <summary>
    /// Shortcut for <c>Default()</c><br/>
    /// 
    /// See also <see cref="Widgets.Spinner"/>.
    /// </summary>
    public readonly Response Spinner() {
        return EguiMarshal.Call<nuint, Response>(EguiFn.egui_ui_Ui_spinner, Ptr);
    }

    /// <summary>
    /// Show a <see cref="RadioButton"/>. It is selected if <c>current_value == selected_value</c>.
    /// If clicked, <paramref name="alternative"/> is assigned to <paramref name="currentValue"/>.
    public readonly Response RadioValue<V>(ref V currentValue, V alternative, Atoms atoms)
    {
        var equal = EqualityComparer<V>.Default.Equals(currentValue, alternative);
        var response = Radio(equal, atoms);
        if (response.Clicked && !equal)
        {
            currentValue = alternative;
            response.MarkChanged();
        }
        return response;
    }

    /// <summary>
    /// Show selectable text. It is selected if <c>current_value == selected_value</c>.
    /// If clicked, <paramref name="alternative"/> is assigned to <paramref name="currentValue"/>.
    public readonly Response SelectableValue<V>(ref V currentValue, V alternative, Atoms atoms)
    {
        var equal = EqualityComparer<V>.Default.Equals(currentValue, alternative);
        var response = SelectableLabel(equal, atoms);
        if (response.Clicked && !equal)
        {
            currentValue = alternative;
            response.MarkChanged();
        }
        return response;
    }

    /// <summary>
    /// A <c>TextEdit</c> for multiple lines. Pressing enter key will create a new line.<br/>
    /// 
    /// See also <c>TextEdit</c>.
    /// </summary>
    public readonly Response TextEditSingleline(string text)
    {
        return Add(TextEdit.Singleline(text));
    }

    /// <inheritdoc cref="TextEditSingleline"/>
    public readonly Response TextEditSingleline(ref string text)
    {
        return Add(TextEdit.Singleline(ref text));
    }

    /// <summary>
    /// A <c>TextEdit</c> for code editing.<br/>
    /// 
    /// This will be multiline, monospace, and will insert tabs instead of moving focus.<br/>
    /// 
    /// See also <c>CodeEditor</c>.
    /// </summary>
    public readonly Response TextEditMultiline(string text)
    {
        return Add(TextEdit.Singleline(text));
    }

    /// <inheritdoc cref="TextEditMultiline"/>
    public readonly Response TextEditMultiline(ref string text)
    {
        return Add(TextEdit.Singleline(ref text));
    }

    /// <summary>
    /// A <c>TextEdit</c> for code editing.<br/>
    /// 
    /// This will be multiline, monospace, and will insert tabs instead of moving focus.<br/>
    /// 
    /// See also <c>CodeEditor</c>.
    /// </summary>
    public readonly Response CodeEditor(string text)
    {
        return Add(TextEdit.Singleline(text));
    }

    /// <inheritdoc cref="CodeEditor"/>
    public readonly Response CodeEditor(ref string text)
    {
        return Add(TextEdit.Singleline(ref text));
    }

    /// <summary>
    /// Create a menu button that when clicked will show the given menu.<br/>
    ///
    /// If called from within a menu this will instead create a button for a sub-menu.<br/>
    /// 
    /// See also <see cref="Close"/> and <see cref="Response.ContextMenu"/>. 
    /// </summary>
    public InnerResponse MenuButton(Atoms atoms, Action<Ui> addContents)
    {
        return new InnerResponse
        {
            Response = MenuButton(atoms, ui =>
            {
                addContents(ui);
                return false;
            }).Response
        };
    }

    /// <inheritdoc cref="MenuButton"/>
    public InnerResponse<R?> MenuButton<R>(Atoms atoms, Func<Ui, R> addContents)
    {
        var (response, inner) = MenuHelpers.IsInMenu(this) ?
            new SubMenuButton(atoms).Ui(this, addContents) :
            new MenuButton(atoms).Ui(this, addContents);

        return new InnerResponse<R?>
        {
            Inner = inner.HasValue ? inner.Value.Inner : default,
            Response = response
        };
    }

    /// <summary>
    /// Create a menu button with an image that when clicked will show the given menu.<br/>
    ///
    /// If called from within a menu this will instead create a button for a sub-menu.<br/>
    /// 
    /// See also <see cref="Close"/> and <see cref="Response.ContextMenu"/>. 
    /// </summary>
    public InnerResponse MenuImageButton(Atoms atoms, Image image, Action<Ui> addContents)
    {
        return new InnerResponse
        {
            Response = MenuImageButton(atoms, image, ui =>
            {
                addContents(ui);
                return false;
            }).Response
        };
    }

    /// <inheritdoc cref="MenuButton"/>
    public InnerResponse<R?> MenuImageButton<R>(Atoms atoms, Image image, Func<Ui, R> addContents)
    {
        var (response, inner) = MenuHelpers.IsInMenu(this) ?
            SubMenuButton.FromButton(Widgets.Button.Image(image).RightText(SubMenuButton.RightArrow)).Ui(this, addContents) :
            Containers.Menu.MenuButton.FromButton(Widgets.Button.Image(image)).Ui(this, addContents);

        return new InnerResponse<R?>
        {
            Inner = inner.HasValue ? inner.Value.Inner : default,
            Response = response
        };
    }
    
    /// <summary>
    /// Create a menu button with an image and a text that when clicked will show the given menu.<br/>
    ///
    /// If called from within a menu this will instead create a button for a sub-menu.<br/>
    /// 
    /// See also <see cref="Close"/> and <see cref="Response.ContextMenu"/>. 
    /// </summary>
    public InnerResponse MenuImageTextButton(Atoms atoms, Image image, WidgetText title, Action<Ui> addContents)
    {
        return new InnerResponse
        {
            Response = MenuImageTextButton(atoms, image, title, ui =>
            {
                addContents(ui);
                return false;
            }).Response
        };
    }

    /// <inheritdoc cref="MenuButton"/>
    public InnerResponse<R?> MenuImageTextButton<R>(Atoms atoms, Image image, WidgetText title, Func<Ui, R> addContents)
    {
        var (response, inner) = MenuHelpers.IsInMenu(this) ?
            SubMenuButton.FromButton(Widgets.Button.ImageAndText(image, title).RightText(SubMenuButton.RightArrow)).Ui(this, addContents) :
            Containers.Menu.MenuButton.FromButton(Widgets.Button.ImageAndText(image, title)).Ui(this, addContents);

        return new InnerResponse<R?>
        {
            Inner = inner.HasValue ? inner.Value.Inner : default,
            Response = response
        };
    }

    /// <summary>
    /// Throws an exception if this is a null object.
    /// </summary>
    internal readonly void AssertInitialized()
    {
        if (Ptr == 0) { throw new NullReferenceException("Ui instance was uninitialized"); }
    }
}