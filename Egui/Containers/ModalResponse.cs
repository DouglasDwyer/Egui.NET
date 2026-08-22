namespace Egui.Containers;

/// <inheritdoc cref="ModalResponse{T}"/>
public struct ModalResponse
{
    /// <inheritdoc cref="ModalResponse{T}.Response"/>
    public required Response Response;

    /// <inheritdoc cref="ModalResponse{T}.BackdropResponse"/>
    public required Response BackdropResponse;

    /// <inheritdoc cref="ModalResponse{T}.IsTopModal"/>
    public required bool IsTopModal;

    /// <inheritdoc cref="ModalResponse{T}.AnyPopupOpen"/>
    public required bool AnyPopupOpen;

    /// <inheritdoc cref="ModalResponse{T}.ShouldClose"/>
    public bool ShouldClose => EguiMarshal.Call<Response, Response, bool, bool, bool>(EguiFn.egui_containers_modal_ModalResponse_should_close, Response, BackdropResponse, IsTopModal, AnyPopupOpen);
}

/// <summary>
/// The response of a modal dialog.
/// </summary>
public struct ModalResponse<T>
{
    /// <summary>
    /// The response of the modal contents
    /// </summary>
    public required Response Response;

    /// <summary>
    /// The response of the modal backdrop.
    ///
    /// A click on this means the user clicked outside the modal,
    /// in which case you might want to close the modal.
    /// </summary>
    public required Response BackdropResponse;

    /// <summary>
    /// The inner response from the content closure
    /// </summary>
    public required T Inner;

    /// <summary>
    /// Is this the topmost modal?
    /// </summary>
    public required bool IsTopModal;

    /// <summary>
    /// Is there any popup open?
    /// We need to check this before the modal contents are shown, so we can know if any popup
    /// was open when checking if the escape key was clicked.
    /// </summary>
    public required bool AnyPopupOpen;

    /// <summary>
    /// Should the modal be closed? Returns true if:<br/>
    /// - the backdrop was clicked<br/>
    /// - this is the topmost modal, no popup is open and the escape key was pressed
    /// </summary>
    public bool ShouldClose => EguiMarshal.Call<Response, Response, bool, bool, bool>(EguiFn.egui_containers_modal_ModalResponse_should_close, Response, BackdropResponse, IsTopModal, AnyPopupOpen);
}