namespace Egui.Containers;

public partial record struct Modal
{
    /// <summary>
    /// Show the modal.
    /// </summary>
    public readonly ModalResponse Show(Context ctx, Action<Ui> addContents)
    {
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var (response, backdropResponse, isTopModal, anyPopupOpen) = EguiMarshal.Call<nuint, Modal, EguiCallback, (Response, Response, bool, bool)>(EguiFn.egui_containers_modal_Modal_show, ctx.Ptr, this, callback);
        return new ModalResponse
        {
            Response = response,
            BackdropResponse = backdropResponse,
            IsTopModal = isTopModal,
            AnyPopupOpen = anyPopupOpen
        };
    }

    /// <inheritdoc cref="Show"/>
    public readonly ModalResponse<R> Show<R>(Context ctx, Func<Ui, R> addContents)
    {
        R result = default!;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var (response, backdropResponse, isTopModal, anyPopupOpen) = EguiMarshal.Call<nuint, Modal, EguiCallback, (Response, Response, bool, bool)>(EguiFn.egui_containers_modal_Modal_show, ctx.Ptr, this, callback);
        return new ModalResponse<R>
        {
            Response = response,
            BackdropResponse = backdropResponse,
            Inner = result,
            IsTopModal = isTopModal,
            AnyPopupOpen = anyPopupOpen
        };
    }
}