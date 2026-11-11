namespace Egui;

public partial record struct Aligned
{
    /// <summary>
    /// Show the contents.
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
        using var callback = new EguiCallback.Pin(innerUi => result = addContents(new Ui(ctx, innerUi)));
        var response = EguiMarshal.Call<nuint, Aligned, EguiCallback, Response>(EguiFn.egui_containers_aligned_Aligned_show, ui.Ptr, this, callback.AsNative());
        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }
}
