namespace Egui.Containers;

public partial record struct Area
{
    public readonly InnerResponse Show(Context ctx, Action<Ui> addContents)
    {
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, Area, EguiCallback, Response>(EguiFn.egui_containers_area_Area_show, ctx.Ptr, this, callback);
        return new InnerResponse
        {
            Response = response
        };
    }

    public readonly InnerResponse<R> Show<R>(Context ctx, Func<Ui, R> addContents)
    {
        R result = default!;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, Area, EguiCallback, Response>(EguiFn.egui_containers_area_Area_show, ctx.Ptr, this, callback);
        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }
}