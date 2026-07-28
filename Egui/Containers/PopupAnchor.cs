namespace Egui.Containers;

public partial record struct PopupAnchor
{
    /// <summary>
    /// Get the rect the popup should be shown relative to.
    /// </summary>
    public readonly Rect? Rect(Id popupId, Context ctx)
    {
        switch (_variantId)
        {
            case 0:
                return _variant0.Value;
            case 1:
                var hoverPos = ctx.PointerHoverPos;
                if (hoverPos.HasValue)
                {
                    return Egui.Rect.FromPos(hoverPos.Value);
                }
                else
                {
                    return null;
                }
            case 2:
                var idPos = Popup.PositionOfId(ctx, popupId);
                if (idPos.HasValue)
                {
                    return Egui.Rect.FromPos(idPos.Value);
                }
                else
                {
                    return null;
                }
            case 3:
                return Egui.Rect.FromPos(_variant3.Value);
            default:
                throw new InvalidDataException($"Unrecognized anchor variant {_variantId}");
        }
    }
}