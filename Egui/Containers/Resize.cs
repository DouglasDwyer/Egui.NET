namespace Egui.Containers;

public partial record struct Resize
{
    public readonly void Show(Ui ui, Action<Ui> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        using var callback = new EguiCallback.Pin(ui => addContents(new Ui(ctx, ui)));
        EguiMarshal.Call(EguiFn.egui_containers_resize_Resize_show, ui.Ptr, this, callback.AsNative());
    }

    public readonly R Show<R>(Ui ui, Func<Ui, R> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        R result = default!;
        using var callback = new EguiCallback.Pin(ui => result = addContents(new Ui(ctx, ui)));
        EguiMarshal.Call(EguiFn.egui_containers_resize_Resize_show, ui.Ptr, this, callback.AsNative());
        return result;
    }
}