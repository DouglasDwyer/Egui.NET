namespace Egui.Containers.Menu;

public partial record struct SubMenuButton
{   
    /// <summary>
    /// The default right arrow symbol: "⏵"
    /// </summary>
    public static readonly string RightArrow = "⏵";

    /// <summary>
    /// Show the menu button.
    /// </summary>
    public readonly (Response, InnerResponse?) Ui(Ui ui, Action<Ui> addContents)
    {
        var (response, maybeInner) = Ui(ui, ui =>
        {
            addContents(ui);
            return false;
        });
        return (response, maybeInner.HasValue ? new InnerResponse
        {
            Response = maybeInner.Value.Response
        } : null);
    }

    /// <inheritdoc cref="Ui"/>
    public readonly (Response, InnerResponse<R>?) Ui<R>(Ui ui, Func<Ui, R> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        R result = default!;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var (response, maybeInner) = EguiMarshal.Call<nuint, SubMenuButton, EguiCallback, (Response, Response?)>(EguiFn.egui_containers_menu_SubMenuButton_ui, ui.Ptr, this, callback);

        return (response, maybeInner.HasValue ? new InnerResponse<R>
        {
            Inner = result,
            Response = response
        } : null);
    }
}