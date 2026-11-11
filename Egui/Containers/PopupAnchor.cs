namespace Egui;

public partial record struct PopupAnchor
{
    /// <summary>
    /// Get the rect the popup should be shown relative to.
    /// </summary>
    public readonly Rect? Rect(Id popupId, Context ctx) => this switch
    {
        ParentRect parentRect => parentRect.Value,
        Pointer => ctx.PointerHoverPos is { } hoverPos ? Egui.Rect.FromPos(hoverPos) : null,
        PointerFixed => Popup.PositionOfId(ctx, popupId) is { } idPos ? Egui.Rect.FromPos(idPos) : null,
        Position position => Egui.Rect.FromPos(position.Value),
    };
}