namespace Egui.Containers;

public partial struct CentralPanel
{
    /// <summary>
    /// Show the panel inside a <see cref="Ui"/>.
    /// </summary>
    public readonly InnerResponse Show(Ui ui, Action<Ui> addContents)
    {
        return new InnerResponse
        {
            Response = Show(ui, ui =>
            {
                addContents(ui);
                return false;
            }).Response
        };
    }

    /// <inheritdoc cref="Show"/>
    public readonly InnerResponse<R> Show<R>(Ui ui, Func<Ui, R> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        R result = default!;
        using var callback = new EguiCallback(innerUi => result = addContents(new Ui(ctx, innerUi)));
        var response = EguiMarshal.Call<nuint, CentralPanel, EguiCallback, Response>(EguiFn.egui_containers_panel_CentralPanel_show, ui.Ptr, this, callback);
        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }
}