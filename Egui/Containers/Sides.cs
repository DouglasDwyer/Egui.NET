namespace Egui.Containers;

public partial record struct Sides
{
    public readonly void Show(Ui ui, Action<Ui> addLeft, Action<Ui> addRight)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        using var leftCallback = new EguiCallback(ui => addLeft(new Ui(ctx, ui)));
        using var rightCallback = new EguiCallback(ui => addRight(new Ui(ctx, ui)));
        EguiMarshal.Call(EguiFn.egui_containers_sides_Sides_show, ui.Ptr, this, leftCallback, rightCallback);
    }

    public readonly (L, R) Show<L, R>(Ui ui, Func<Ui, L> addLeft, Func<Ui, R> addRight)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        L lResult = default!;
        R rResult = default!;
        using var leftCallback = new EguiCallback(ui => lResult = addLeft(new Ui(ctx, ui)));
        using var rightCallback = new EguiCallback(ui => rResult = addRight(new Ui(ctx, ui)));
        EguiMarshal.Call(EguiFn.egui_containers_sides_Sides_show, ui.Ptr, this, leftCallback, rightCallback);
        return (lResult, rResult);
    }
}