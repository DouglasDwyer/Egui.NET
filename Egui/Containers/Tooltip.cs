namespace Egui.Containers;

public ref partial struct Tooltip
{
    public Popup Popup;

    /// <summary>
    /// The layer of the parent widget.
    /// </summary>
    private LayerId _parentLayer;

    /// <summary>
    /// The id of the widget that owns this tooltip.
    /// </summary>
    private Id _parentWidget;

    /// <summary>
    /// Show a tooltip that is always open.
    /// </summary>
    public static Tooltip AlwaysOpen(Context ctx, LayerId parentLayer, Id parentWidget, PopupAnchor anchor)
    {
        var width = ctx.GlobalStyle.Spacing.TooltipWidth;
        return new Tooltip
        {
            Popup = new Popup(parentWidget, ctx, anchor, parentLayer)
                .Kind(PopupKind.Tooltip)
                .Gap(4.0f)
                .Width(width)
                .Sense(Sense.Hover),
            _parentLayer = parentLayer,
            _parentWidget = parentWidget
        };
    }

    /// <summary>
    /// Show a tooltip for a widget. Always open (as long as this function is called).
    /// </summary>
    public static Tooltip ForWidget(Response response)
    {
        var popup = Popup.FromResponse(response)
            .Kind(PopupKind.Tooltip)
            .Gap(4.0f)
            .Width(response.Ctx.GlobalStyle.Spacing.TooltipWidth)
            .Sense(Sense.Hover);
        return new Tooltip
        {
            Popup = popup,
            _parentLayer = response.LayerId,
            _parentWidget = response.Id
        };
    }

    /// <summary>
    /// Show a tooltip when hovering an enabled widget.
    /// </summary>
    public static Tooltip ForEnabled(Response response)
    {
        var tooltip = ForWidget(response);
        tooltip.Popup = tooltip.Popup.Open(response.Enabled && ShouldShowTooltip(response, true));
        return tooltip;
    }

    /// <summary>
    /// Show a tooltip when hovering a disabled widget.
    /// </summary>
    public static Tooltip ForDisabled(Response response)
    {
        var tooltip = ForWidget(response);
        tooltip.Popup = tooltip.Popup.Open(!response.Enabled && ShouldShowTooltip(response, true));
        return tooltip;
    }

    /// <summary>
    /// Show the tooltip at the pointer position.
    /// </summary>
    public readonly Tooltip AtPointer()
    {
        var result = this;
        result.Popup = result.Popup.AtPointer();
        return result;
    }

    /// <summary>
    /// Set the gap between the tooltip and the anchor<br/>
    ///
    /// Default: 5.0
    /// </summary>
    public readonly Tooltip Gap(float gap)
    {
        var result = this;
        result.Popup = result.Popup.Gap(gap);
        return result;
    }

    /// <summary>
    /// Set the layout of the tooltip
    /// </summary>
    public readonly Tooltip Layout(Layout layout)
    {
        var result = this;
        result.Popup = result.Popup.Layout(layout);
        return result;
    }

    /// <summary>
    /// Set the layout of the tooltip
    /// </summary>
    public readonly Tooltip Width(float width)
    {
        var result = this;
        result.Popup = result.Popup.Width(width);
        return result;
    }

    /// <summary>
    /// Show the tooltip
    /// </summary>
    public readonly InnerResponse? Show(Action<Ui> content)
    {
        var result = Show(ui => { content(ui); return false; });

        if (result.HasValue)
        {
            return new InnerResponse
            {
                Response = result.Value.Response
            };
        }
        else
        {
            return null;
        }
    }

    /// <inheritdoc cref="Show"/>
    public readonly InnerResponse<R>? Show<R>(Func<Ui, R> content)
    {
        return Popup.ShowTooltip(_parentLayer, _parentWidget, content);
    }
}