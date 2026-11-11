namespace Egui;

public partial record struct CollapsingHeader
{
    /// <summary>
    /// Shows a collapsing header with a custom header UI, which starts collapsed.
    /// </summary>
    public static void Custom(Ui ui, IdSalt idSalt, Action<Ui> addHeader, Action<Ui> addBody)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        using var header = new EguiCallback.Pin(ui => addHeader(new Ui(ctx, ui)));
        using var body = new EguiCallback.Pin(ui => addBody(new Ui(ctx, ui)));
        EguiMarshal.Call(EguiFn.egui_containers_collapsing_header_CollapsingHeader_custom, ui.Ptr, idSalt, header.AsNative(), body.AsNative());
    }

    public readonly CollapsingResponse Show(Ui ui, Action<Ui> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        using var callback = new EguiCallback.Pin(ui => addContents(new Ui(ctx, ui)));
        var (headerResponse, bodyResponse, openness) = EguiMarshal.Call<nuint, CollapsingHeader, EguiCallback, (Response, Response?, float)>(EguiFn.egui_containers_collapsing_header_CollapsingHeader_show, ui.Ptr, this, callback.AsNative());
        return new CollapsingResponse
        {
            HeaderResponse = headerResponse,
            BodyResponse = bodyResponse,
            Openness = openness
        };
    }

    public readonly CollapsingResponse<R> Show<R>(Ui ui, Func<Ui, R> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        R? result = default!;
        using var callback = new EguiCallback.Pin(ui => result = addContents(new Ui(ctx, ui)));
        var (headerResponse, bodyResponse, openness) = EguiMarshal.Call<nuint, CollapsingHeader, EguiCallback, (Response, Response?, float)>(EguiFn.egui_containers_collapsing_header_CollapsingHeader_show, ui.Ptr, this, callback.AsNative());
        return new CollapsingResponse<R>
        {
            HeaderResponse = headerResponse,
            BodyResponse = bodyResponse,
            BodyReturned = result,
            Openness = openness
        };
    }

    public readonly CollapsingResponse ShowUnindented(Ui ui, Action<Ui> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        using var callback = new EguiCallback.Pin(ui => addContents(new Ui(ctx, ui)));
        var (headerResponse, bodyResponse, openness) = EguiMarshal.Call<nuint, CollapsingHeader, EguiCallback, (Response, Response?, float)>(EguiFn.egui_containers_collapsing_header_CollapsingHeader_show_unindented, ui.Ptr, this, callback.AsNative());
        return new CollapsingResponse
        {
            HeaderResponse = headerResponse,
            BodyResponse = bodyResponse,
            Openness = openness
        };
    }

    public readonly CollapsingResponse<R> ShowUnindented<R>(Ui ui, Func<Ui, R> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        R? result = default!;
        using var callback = new EguiCallback.Pin(ui => result = addContents(new Ui(ctx, ui)));
        var (headerResponse, bodyResponse, openness) = EguiMarshal.Call<nuint, CollapsingHeader, EguiCallback, (Response, Response?, float)>(EguiFn.egui_containers_collapsing_header_CollapsingHeader_show_unindented, ui.Ptr, this, callback.AsNative());
        return new CollapsingResponse<R>
        {
            HeaderResponse = headerResponse,
            BodyResponse = bodyResponse,
            BodyReturned = result,
            Openness = openness
        };
    }
}