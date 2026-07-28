namespace Egui.Containers;

public partial record struct Frame
{
    /// <summary>
    /// Show the given ui surrounded by this frame.
    /// </summary>
    public readonly InnerResponse Show(Ui ui, Action<Ui> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, Frame, EguiCallback, Response>(EguiFn.egui_containers_frame_Frame_show, ui.Ptr, this, callback);
        return new InnerResponse
        {
            Response = response
        };
    }

    /// <inheritdoc cref="Show"/>
    public readonly InnerResponse<R> Show<R>(Ui ui, Func<Ui, R> addContents)
    {
        ui.AssertInitialized();
        var ctx = ui.Ctx;
        R result = default!;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, Frame, EguiCallback, Response>(EguiFn.egui_containers_frame_Frame_show, ui.Ptr, this, callback);
        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }
}