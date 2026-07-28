namespace Egui;

public partial record struct MenuBar
{
    /// <summary>
    /// Show the menu bar.
    /// </summary>
    public readonly InnerResponse Ui(Ui ui, Action<Ui> addContents)
    {
        return new InnerResponse
        {
            Response = Ui(ui, ui =>
            {
                addContents(ui);
                return false;
            }).Response
        };
    }

    /// <inheritdoc cref="Ui"/>
    public readonly InnerResponse<R> Ui<R>(Ui ui, Func<Ui, R> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        R result = default!;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, MenuBar, EguiCallback, Response>(EguiFn.egui_containers_menu_MenuBar_ui, ui.Ptr, this, callback);
        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }
}