namespace Egui.Containers.Menu;

public partial record struct SubMenu
{
    /// <summary>
    /// Show the submenu.
    /// </summary>
    public readonly InnerResponse? Show(Ui ui, Response buttonResponse, Action<Ui> content)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        using var callback = new EguiCallback(ui => content(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, SubMenu, Response, EguiCallback, Response?>(EguiFn.egui_containers_menu_SubMenu_show, ui.Ptr, this, buttonResponse, callback);
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

    /// <inheritdoc cref="Show"/>
    public InnerResponse<R>? Show<R>(Ui ui, Response buttonResponse, Func<Ui, R> content)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        R result = default!;
        using var callback = new EguiCallback(ui => result = content(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, SubMenu, Response, EguiCallback, Response?>(EguiFn.egui_containers_menu_SubMenu_show, ui.Ptr, this, buttonResponse, callback);
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
}