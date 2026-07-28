namespace Egui.Containers;

public partial record struct CollapsingHeader
{
    public readonly CollapsingResponse Show(Ui ui, Action<Ui> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var (headerResponse, bodyResponse, openness) = EguiMarshal.Call<nuint, CollapsingHeader, EguiCallback, (Response, Response?, float)>(EguiFn.egui_containers_collapsing_header_CollapsingHeader_show, ui.Ptr, this, callback);
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
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var (headerResponse, bodyResponse, openness) = EguiMarshal.Call<nuint, CollapsingHeader, EguiCallback, (Response, Response?, float)>(EguiFn.egui_containers_collapsing_header_CollapsingHeader_show, ui.Ptr, this, callback);
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
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var (headerResponse, bodyResponse, openness) = EguiMarshal.Call<nuint, CollapsingHeader, EguiCallback, (Response, Response?, float)>(EguiFn.egui_containers_collapsing_header_CollapsingHeader_show_unindented, ui.Ptr, this, callback);
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
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var (headerResponse, bodyResponse, openness) = EguiMarshal.Call<nuint, CollapsingHeader, EguiCallback, (Response, Response?, float)>(EguiFn.egui_containers_collapsing_header_CollapsingHeader_show_unindented, ui.Ptr, this, callback);
        return new CollapsingResponse<R>
        {
            HeaderResponse = headerResponse,
            BodyResponse = bodyResponse,
            BodyReturned = result,
            Openness = openness
        };
    }
}