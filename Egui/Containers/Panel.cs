namespace Egui.Containers;

public partial struct Panel
{
    /// <summary>
    /// Show the panel inside a <see cref="Ui"/>.
    /// </summary>
    public readonly InnerResponse Show(Ui ui, Action<Ui> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        using var callback = new EguiCallback(innerUi => addContents(new Ui(ctx, innerUi)));
        var response = EguiMarshal.Call<nuint, Panel, EguiCallback, Response>(EguiFn.egui_containers_panel_Panel_show, ui.Ptr, this, callback);
        return new InnerResponse
        {
            Response = response
        };
    }

    /// <inheritdoc cref="Show"/>
    public readonly InnerResponse<R> Show<R>(Ui ui, Func<Ui, R> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        R result = default!;
        using var callback = new EguiCallback(innerUi => result = addContents(new Ui(ctx, innerUi)));
        var response = EguiMarshal.Call<nuint, Panel, EguiCallback, Response>(EguiFn.egui_containers_panel_Panel_show, ui.Ptr, this, callback);
        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }

    /// <summary>
    /// Show the panel if <paramref name="isExpanded"/> is <c>true</c>, otherwise hide it, with a slide animation in between.<br/>
    /// <paramref name="isExpanded"/> may be flipped to <c>false</c> when the user drags the resize handle past the panel's minimum size, and back to <c>true</c> if the user drags the handle outward while the panel is closed.
    /// </summary>
    public readonly InnerResponse? ShowCollapsible(Ui ui, ref bool isExpanded, Action<Ui> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        using var callback = new EguiCallback(innerUi => addContents(new Ui(ctx, innerUi)));
        var (response, expanded) = EguiMarshal.Call<nuint, bool, Panel, EguiCallback, (Response?, bool)>(EguiFn.egui_containers_panel_Panel_show_collapsible, ui.Ptr, isExpanded, this, callback);
        isExpanded = expanded;

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

    /// <inheritdoc cref="ShowCollapsible"/>
    public readonly InnerResponse<R>? ShowCollapsible<R>(Ui ui, ref bool isExpanded, Func<Ui, R> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        R result = default!;
        using var callback = new EguiCallback(innerUi => result = addContents(new Ui(ctx, innerUi)));
        var (response, expanded) = EguiMarshal.Call<nuint, bool, Panel, EguiCallback, (Response?, bool)>(EguiFn.egui_containers_panel_Panel_show_collapsible, ui.Ptr, isExpanded, this, callback);
        isExpanded = expanded;

        if (response.HasValue)
        {
            return new InnerResponse<R>
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
    /// Show either a collapsed or expanded panel, with a nice slide animation between.<br/>
    /// <paramref name="addContents"/> receives <c>expanded = true</c> whenever the expanded panel is rendered (including mid-animation), and <c>false</c> for the collapsed view.
    /// </summary>
    public unsafe static InnerResponse ShowSwitched(Ui ui, ref bool isExpanded, Panel collapsedPanel, Panel expandedPanel, Action<Ui, bool> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        using var callback = new EguiCallback(data =>
        {
            EguiSwitchedUi switchedUi = *(EguiSwitchedUi*)data;
            addContents(new Ui(ctx, switchedUi.ui), switchedUi.expanded);
        });

        var (response, expanded) = EguiMarshal.Call<nuint, bool, Panel, Panel, EguiCallback, (Response, bool)>(EguiFn.egui_containers_panel_Panel_show_switched, ui.Ptr, isExpanded, collapsedPanel, expandedPanel, callback);
        isExpanded = expanded;
        return new InnerResponse
        {
            Response = response
        };
    }

    /// <inheritdoc cref="ShowSwitched"/>
    public unsafe static InnerResponse<R> ShowSwitched<R>(Ui ui, ref bool isExpanded, Panel collapsedPanel, Panel expandedPanel, Func<Ui, bool, R> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        R result = default!;
        using var callback = new EguiCallback(data =>
        {
            EguiSwitchedUi switchedUi = *(EguiSwitchedUi*)data;
            result = addContents(new Ui(ctx, switchedUi.ui), switchedUi.expanded);
        });

        var (response, expanded) = EguiMarshal.Call<nuint, bool, Panel, Panel, EguiCallback, (Response, bool)>(EguiFn.egui_containers_panel_Panel_show_switched, ui.Ptr, isExpanded, collapsedPanel, expandedPanel, callback);
        isExpanded = expanded;
        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }
}
