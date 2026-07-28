namespace Egui.Containers;

public partial record struct CollapsingState
{
    /// <summary>
    /// Show body if we are open, with a nice animation between closed and open.<br/>
    /// Indent the body to show it belongs to the header.<br/>
    ///
    /// Will also store the state.
    /// </summary>
    public InnerResponse? ShowBodyIndented(Response headerResponse, Ui ui, Action<Ui> addBody)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        using var callback = new EguiCallback(ui => addBody(new Ui(ctx, ui)));
        var (response, newState) = EguiMarshal.Call<nuint, CollapsingState, Response, EguiCallback, (Response?, CollapsingState)>(EguiFn.egui_containers_collapsing_header_CollapsingState_show_body_indented, ui.Ptr, this, headerResponse, callback);
        this = newState;
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

    /// <inheritdoc cref="ShowBodyIndented"/>
    public InnerResponse<R>? ShowBodyIndented<R>(Response headerResponse, Ui ui, Func<Ui, R> addBody)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        R result = default!;
        using var callback = new EguiCallback(ui => result = addBody(new Ui(ctx, ui)));
        var (response, newState) = EguiMarshal.Call<nuint, CollapsingState, Response, EguiCallback, (Response?, CollapsingState)>(EguiFn.egui_containers_collapsing_header_CollapsingState_show_body_indented, ui.Ptr, this, headerResponse, callback);
        this = newState;
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
    /// how body if we are open, with a nice animation between closed and open.<br/>
    ///
    /// Will also store the state.
    /// </summary>
    public InnerResponse? ShowBodyUnindented(Ui ui, Action<Ui> addBody)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        using var callback = new EguiCallback(ui => addBody(new Ui(ctx, ui)));
        var (response, newState) = EguiMarshal.Call<nuint, CollapsingState, EguiCallback, (Response?, CollapsingState)>(EguiFn.egui_containers_collapsing_header_CollapsingState_show_body_unindented, ui.Ptr, this, callback);
        this = newState;
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

    /// <inheritdoc cref="ShowBodyUnindented"/>
    public InnerResponse<R>? ShowBodyUnindented<R>(Ui ui, Func<Ui, R> addBody)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        R result = default!;
        using var callback = new EguiCallback(ui => result = addBody(new Ui(ctx, ui)));
        var (response, newState) = EguiMarshal.Call<nuint, CollapsingState, EguiCallback, (Response?, CollapsingState)>(EguiFn.egui_containers_collapsing_header_CollapsingState_show_body_unindented, ui.Ptr, this, callback);
        this = newState;
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
    /// how body if we are open, with a nice animation between closed and open.<br/>
    ///
    /// Will also store the state.
    /// </summary>
    public readonly HeaderResponse ShowHeader(Ui ui, Action<Ui> addHeader)
    {
        return ShowHeader(ui, ui =>
        {
            addHeader(ui);
            return false;
        }).WithoutInner();
    }

    /// <inheritdoc cref="ShowHeader"/>
    public readonly HeaderResponse<R> ShowHeader<R>(Ui ui, Func<Ui, R> addHeader)
    {
        R result = default!;
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        using var callback = new EguiCallback(ui => result = addHeader(new Ui(ctx, ui)));
        var (toggleResponse, headerResponse, newState) = EguiMarshal.Call<nuint, CollapsingState, EguiCallback, (Response, Response, CollapsingState)>(EguiFn.egui_containers_collapsing_header_CollapsingState_show_header, ui.Ptr, this, callback);
        return new HeaderResponse<R>(this, ui, toggleResponse, new InnerResponse<R>
        {
            Inner = result,
            Response = headerResponse
        });
    }

    /// <summary>
    /// Paint this <see cref="CollapsingState"/>'s toggle button. Takes an <c>IconPainter</c> as the icon.
    /// </summary>
    public Response ShowToggleButton(Ui ui, Action<Ui, float, Response> iconFn)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        using var callback = new EguiCallback(paramPack =>
        {
            var (ui, openness, response) = EguiMarshal.Call<nuint, (nuint, float, Response)>(EguiFn.egui_containers_collapsing_header_EguiCollapsingStateShowToggleButtonParams_unpack, paramPack);
            iconFn(new Ui(ctx, ui), openness, response);
        });
        return EguiMarshal.Call<nuint, CollapsingState, EguiCallback, Response>(EguiFn.egui_containers_collapsing_header_CollapsingState_show_toggle_button, ui.Ptr, this, callback);
    }
}