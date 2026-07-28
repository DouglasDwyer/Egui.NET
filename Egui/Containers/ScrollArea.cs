namespace Egui.Containers;

public partial record struct ScrollArea
{
    /// <summary>
    /// Show the <see cref="ScrollArea"/>, and add the contents to the viewport.<br/>
    /// If the inner area can be very long, consider using <see cref="ShowRows"/> instead.
    /// </summary>
    public readonly ScrollAreaOutput Show(Ui ui, Action<Ui> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var (id, state, contentSize, innerRect) = EguiMarshal.Call<nuint, ScrollArea, EguiCallback, (Id, State, EVec2, Rect)>(EguiFn.egui_containers_scroll_area_ScrollArea_show, ui.Ptr, this, callback);
        return new ScrollAreaOutput
        {
            Id = id,
            State = state,
            ContentSize = contentSize,
            InnerRect = innerRect
        };
    }

    /// <inheritdoc cref="Show"/>
    public readonly ScrollAreaOutput<R> Show<R>(Ui ui, Func<Ui, R> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        R result = default!;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var (id, state, contentSize, innerRect) = EguiMarshal.Call<nuint, ScrollArea, EguiCallback, (Id, State, EVec2, Rect)>(EguiFn.egui_containers_scroll_area_ScrollArea_show, ui.Ptr, this, callback);
        return new ScrollAreaOutput<R>
        {
            Inner = result,
            Id = id,
            State = state,
            ContentSize = contentSize,
            InnerRect = innerRect
        };
    }

    /// <summary>
    /// Efficiently show only the visible part of a large number of rows.
    /// </summary>
    public unsafe readonly ScrollAreaOutput ShowRows(Ui ui, float rowHeightSansSpacing, nuint totalRows, Action<Ui, nuint, nuint> addContents)
    {
        return ShowRows(ui, rowHeightSansSpacing, totalRows, (ui, start, end) =>
        {
            addContents(ui, start, end);
            return false;
        }).WithoutInner();
    }

    /// <inheritdoc cref="ShowRows"/>
    public unsafe readonly ScrollAreaOutput<R> ShowRows<R>(Ui ui, float rowHeightSansSpacing, nuint totalRows, Func<Ui, nuint, nuint, R> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        R result = default!;
        using var callback = new EguiCallback(data =>
        {
            var unpackedData = *(EguiScrollAreaShowRowsParams*)data;
            result = addContents(new Ui(ctx, unpackedData.ui), unpackedData.start, unpackedData.end);
        });
        var (id, state, contentSize, innerRect) = EguiMarshal.Call<nuint, ScrollArea, float, nuint, EguiCallback, (Id, State, EVec2, Rect)>(EguiFn.egui_containers_scroll_area_ScrollArea_show_rows, ui.Ptr, this, rowHeightSansSpacing, totalRows, callback);
        return new ScrollAreaOutput<R>
        {
            Id = id,
            State = state,
            Inner = result,
            ContentSize = contentSize,
            InnerRect = innerRect
        };
    }

    /// <summary>
    /// This can be used to only paint the visible part of the contents.
    /// <paramref name="addContents"/> is given the viewport rectangle, which is the relative view of the content. So if the passed rect has min = zero, then show the top left content (the user has not scrolled).
    /// </summary>
    public unsafe readonly ScrollAreaOutput ShowViewport(Ui ui, Action<Ui, Rect> addContents)
    {
        return ShowViewport(ui, (ui, rect) =>
        {
            addContents(ui, rect);
            return false;
        }).WithoutInner();
    }

    /// <inheritdoc cref="ShowViewport"/>
    public unsafe readonly ScrollAreaOutput<R> ShowViewport<R>(Ui ui, Func<Ui, Rect, R> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        R result = default!;
        using var callback = new EguiCallback(data =>
        {
            var unpackedData = *(EguiScrollAreaShowViewportParams*)data;
            result = addContents(new Ui(ctx, unpackedData.ui), Rect.FromMinMax(new EPos2(unpackedData.min_x, unpackedData.min_y), new EPos2(unpackedData.max_x, unpackedData.max_y)));
        });
        var (id, state, contentSize, innerRect) = EguiMarshal.Call<nuint, ScrollArea, EguiCallback, (Id, State, EVec2, Rect)>(EguiFn.egui_containers_scroll_area_ScrollArea_show_viewport, ui.Ptr, this, callback);
        return new ScrollAreaOutput<R>
        {
            Id = id,
            State = state,
            Inner = result,
            ContentSize = contentSize,
            InnerRect = innerRect
        };
    }
}