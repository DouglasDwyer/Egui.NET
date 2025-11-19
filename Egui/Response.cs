namespace Egui;

/// <summary>
/// The result of adding a widget to a <c>Ui</c>.<br/>
///
/// A <c>Response</c> lets you know whether a widget is being hovered, clicked or dragged.
/// It also lets you easily show a tooltip on hover.<br/>
///
/// Whenever something gets added to a <c>Ui</c>, a <c>Response</c> object is returned.
/// <c>Ui.add</c> returns a <c>Response</c>, as does <c>Ui.button</c>, and all similar shortcuts.<br/>
///
/// ⚠️ The <c>Response</c> contains a clone of <c>Context</c>, and many methods lock the <c>Context</c>.
/// It can therefore be a deadlock to use <c>Context</c> from within a context-locking closures,
/// such as <c>Input</c>.
/// </summary>
public partial struct Response : IEquatable<Response>
{
    /// <summary>
    /// Used for optionally showing a tooltip and checking for more interactions.
    /// </summary>
    public Context Ctx;
    /// <summary>
    /// Which layer the widget is part of.
    /// </summary>
    public Egui.LayerId LayerId;
    /// <summary>
    /// The <c>Id</c> of the widget/area this response pertains.
    /// </summary>
    public Egui.Id Id;
    /// <summary>
    /// The area of the screen we are talking about.
    /// </summary>
    public Egui.Rect Rect;
    /// <summary>
    /// The rectangle sensing interaction.<br/>
    ///
    /// This is sometimes smaller than <c>Rect</c> because of clipping
    /// (e.g. when inside a scroll area).
    /// </summary>
    public Egui.Rect InteractRect;
    /// <summary>
    /// The senses (click and/or drag) that the widget was interested in (if any).<br/>
    ///
    /// Note: if <c>Enabled</c> is <c>False</c>, then
    /// the widget _effectively_ doesn't sense anything,
    /// but can still have the same <c>Sense</c>.
    /// This is because the sense informs the styling of the widget,
    /// but we don't want to change the style when a widget is disabled
    /// (that is handled by the <c>Painter</c> directly).
    /// </summary>
    public Egui.Sense Sense;
    private Egui.EPos2? _interactPointerPos;
    /// <summary>
    /// The intrinsic / desired size of the widget.<br/>
    ///
    /// This is the size that a non-wrapped, non-truncated, non-justified version of the widget
    /// would have.<br/>
    ///
    /// If this is <c>None</c>, use <c>Rect</c> instead.<br/>
    ///
    /// At the time of writing, this is only used by external crates
    /// for improved layouting.
    /// See for instance <c>EguiFlex</c>.
    /// </summary>
    public Egui.EVec2? IntrinsicSize;
    private Egui.Flags _flags;

    /// <summary>
    /// Response to secondary clicks (right-clicks) by showing the given menu.<br/>
    /// Make sure the widget senses clicks (e.g. <see cref="Button"/>  does, <see cref="Label"/> does not).
    /// </summary>
    public readonly InnerResponse? ContextMenu(Action<Ui> addContents)
    {
        return Popup.ContextMenu(this).Show(addContents);
    }

    /// <summary>
    /// Always show this tooltip, even if disabled and the user isn't hovering it.<br/>
    /// 
    /// This can be used to give attention to a widget during a tutorial.
    /// </summary>
    public readonly void ShowTooltipUi(Action<Ui> addContents)
    {
        Popup.FromResponse(this)
            .Kind(PopupKind.Tooltip)
            .Show(addContents);
    }

    /// <summary>
    /// Show this UI if the widget was hovered (i.e. a tooltip).<br/>
    /// The text will not be visible if the widget is not enabled. For that, use <see cref="OnDisabledHoverUi"/> instead.<br/>
    /// If you call this multiple times the tooltips will stack underneath the previous ones.<br/>
    /// The widget can contain interactive widgets, such as buttons and links. If so, it will stay open as the user moves their pointer over it. By default, the text of a tooltip is NOT selectable (i.e. interactive), but you can change this by setting <see cref="Interaction.SelectableLabels"/> from within the tooltip.
    /// </summary>
    public readonly Response OnHoverUi(Action<Ui> addContents)
    {
        Tooltip.ForEnabled(this).Show(addContents);
        return this;
    }

    /// <summary>
    /// Like <see cref="OnHoverUi"/>, but show the ui next to cursor. 
    /// </summary>
    public readonly Response OnHoverUiAtPointer(Action<Ui> addContents)
    {
        Tooltip.ForEnabled(this)
            .AtPointer()
            .Gap(12.0f)
            .Show(addContents);
        return this;
    }

    /// <summary>
    /// Show this UI when hovering if the widget is disabled.
    /// </summary>
    public readonly Response OnDisabledHoverUi(Action<Ui> addContents)
    {
        Tooltip.ForDisabled(this).Show(addContents);
        return this;
    }

    /// <summary>
    /// For accessibility.
    ///
    /// Call after interacting and potential calls to <see cref="MarkChanged"/>.
    /// </summary>
    public void WidgetInfo(Func<WidgetInfo> makeInfo)
    {
        if (Clicked || DoubleClicked || TripleClicked || GainedFocus || Changed)
        {
            EguiMarshal.Call(EguiFn.egui_response_Response_widget_info, this, makeInfo());
        }
    }

    internal static void Serialize(BincodeSerializer serializer, Response value) => value.Serialize(serializer);

    internal void Serialize(BincodeSerializer serializer)
    {
        serializer.increase_container_depth();
        serializer.serialize_u64(Ctx.Id);
        LayerId.Serialize(serializer);
        Id.Serialize(serializer);
        Rect.Serialize(serializer);
        InteractRect.Serialize(serializer);
        Sense.Serialize(serializer);
        Egui.TraitHelpers.serialize_option_EPos2(_interactPointerPos, serializer);
        Egui.TraitHelpers.serialize_option_EVec2(IntrinsicSize, serializer);
        _flags.Serialize(serializer);
        serializer.decrease_container_depth();
    }

    internal static Response Deserialize(BincodeDeserializer deserializer)
    {
        deserializer.increase_container_depth();
        Response obj = default;
        obj.Ctx = Context.FromId((nuint)deserializer.deserialize_u64());
        EguiMarshal.Call(EguiFn.egui_context_Context_ref_decrement, obj.Ctx.Ptr);
        obj.LayerId = Egui.LayerId.Deserialize(deserializer);
        obj.Id = Egui.Id.Deserialize(deserializer);
        obj.Rect = Egui.Rect.Deserialize(deserializer);
        obj.InteractRect = Egui.Rect.Deserialize(deserializer);
        obj.Sense = Egui.Sense.Deserialize(deserializer);
        obj._interactPointerPos = Egui.TraitHelpers.deserialize_option_EPos2(deserializer);
        obj.IntrinsicSize = Egui.TraitHelpers.deserialize_option_EVec2(deserializer);
        obj._flags = Egui.Flags.Deserialize(deserializer);

        deserializer.decrease_container_depth();
        return obj;
    }
    public override bool Equals(object? obj) => obj is Response other && Equals(other);

    public static bool operator ==(Response left, Response right) => Equals(left, right);

    public static bool operator !=(Response left, Response right) => !Equals(left, right);

    /// <inheritdoc/>
    public bool Equals(Response other)
    {
        if (Ctx != other.Ctx) return false;
        if (LayerId != other.LayerId) return false;
        if (Id != other.Id) return false;
        if (Rect != other.Rect) return false;
        if (InteractRect != other.InteractRect) return false;
        if (Sense != other.Sense) return false;
        if (_interactPointerPos != other._interactPointerPos) return false;
        if (IntrinsicSize != other.IntrinsicSize) return false;
        if (_flags != other._flags) return false;
        return true;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        unchecked
        {
            int value = 7;
            value = 31 * value + LayerId.GetHashCode();
            value = 31 * value + Id.GetHashCode();
            value = 31 * value + Rect.GetHashCode();
            value = 31 * value + InteractRect.GetHashCode();
            value = 31 * value + Sense.GetHashCode();
            value = 31 * value + _interactPointerPos.GetHashCode();
            value = 31 * value + IntrinsicSize.GetHashCode();
            value = 31 * value + _flags.GetHashCode();
            return value;
        }
    }

    /// <summary>
    /// Converts a response to its associated popup anchor.
    /// </summary>
    /// <param name="response">The response to convert.</param>
    public static implicit operator PopupAnchor(Response response)
    {
        var widgetRect = response.InteractRect;

        var global = response.Ctx.LayerTransformToGlobal(response.LayerId);
        if (global.HasValue)
        {
            widgetRect = global.Value.MulRect(widgetRect);
        }

        return new PopupAnchor.ParentRect()
        {
            Value = widgetRect
        };
    }

    /// <summary>
    /// See <see cref="Union"/>.
    ///
    /// To summarize the response from many widgets you can use this pattern
    /// </summary>
    public static Response operator |(Response lhs, Response rhs) => lhs.Union(rhs);
}