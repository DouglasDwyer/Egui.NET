using System.Collections.Immutable;

namespace Egui.Containers;

/// <summary>
/// A popup container.
/// </summary>
public ref partial struct Popup
{
    /// <summary>
    /// Return the anchor rect of the popup.
    /// 
    /// Returns <c>null</c> if the anchor is <see cref="PopupAnchor.Pointer"/>. 
    /// </summary>
    public Rect? AnchorRect => _anchor.Rect(_id, Ctx);

    /// <summary>
    /// Get the expected size of the popup.
    /// </summary>
    public EVec2? ExpectedSize => AreaState.Load(Ctx, _id)?.Size;

    /// <summary>
    /// Calculate the best alignment for the popup, based on the last size and screen rect.
    /// </summary>
    public RectAlign BestAlign
    {
        get
        {
            var expectedPopupSize = ExpectedSize.GetValueOrDefault(new EVec2(_width.GetValueOrDefault(), 0));
            var maybeAnchorRect = _anchor.Rect(_id, Ctx);
            if (!maybeAnchorRect.HasValue)
            {
                return _rectAlign;
            }

            var anchorRect = maybeAnchorRect.Value;
            return RectAlign.FindBestAlign(new[] { _rectAlign }
                .Concat(_alternativeAligns ?? _rectAlign.Symmetries.Concat(RectAlign.MenuAligns)),
                Ctx.ContentRect,
                anchorRect,
                _gap,
                expectedPopupSize).GetValueOrDefault();
        }
    }

    /// <summary>
    /// Get the expected rect the popup will be shown in.
    /// 
    /// Returns <c>null</c> if the popup wasn't shown before or anchor is <see cref="PopupAnchor.Pointer"/> and
    /// there is no pointer. 
    /// </summary>
    public Rect? PopupRect
    {
        get
        {
            var size = ExpectedSize;
            var anchor = AnchorRect;
            if (size.HasValue && anchor.HasValue)
            {
                return BestAlign.AlignRect(anchor.Value, size.Value, _gap);
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Is the popup open?
    /// </summary>
    public readonly bool IsOpen
    {
        get
        {
            switch (_openKind)
            {
                case OpenKind.Open:
                    return true;
                case OpenKind.Closed:
                    return false;
                case OpenKind.Bool:
                    return _open;
                default:
                    return IsIdOpen(Ctx, _id);
            }
        }
    }

    private Id _id;
    public readonly Context Ctx;
    private PopupAnchor _anchor;
    private RectAlign _rectAlign;
    private ImmutableArray<RectAlign>? _alternativeAligns;
    private LayerId _layerId;
    private OpenKind _openKind;
    private SetOpenCommand? _memoryCommand;
    private ref bool _open;
    private PopupCloseBehavior _closeBehavior;
    private UiStackInfo? _info;
    private PopupKind _kind;
    private float _gap;
    private float? _width;
    private Sense _sense;
    private Layout _layout;
    private Frame? _frame;
    private Style? _style;
    private StyleMode _styleMode;


    /// <summary>
    /// Create a new popup
    /// </summary>
    public Popup(Id id, Context ctx, PopupAnchor anchor, LayerId layerId)
    {
        _id = id;
        Ctx = ctx;
        _anchor = anchor;
        _openKind = OpenKind.Open;
        _info = null;
        _kind = PopupKind.Popup;
        _layerId = layerId;
        _rectAlign = RectAlign.BottomStart;
        _alternativeAligns = null;
        _width = null;
        _sense = Egui.Sense.Click;
        _layout = new Layout();
        _frame = null;
        _style = null;
        _styleMode = StyleMode.None;
    }

    /// <summary>
    /// Show a popup relative to some widget.
    /// The popup will  always be open.
    /// 
    /// See <see cref="Menu"/> and <see cref="ContextMenu"/> for common use cases.  
    /// </summary>
    public static Popup FromResponse(Response response)
    {
        var result = new Popup(DefaultResponseId(response), response.Ctx, response, response.LayerId);
        return result;
    }

    /// <summary>
    /// Show a popup relative to some widget,
    /// toggling the open state based on the widget's click state.
    /// 
    /// See <see cref="Menu"/> and <see cref="ContextMenu"/> for common use cases.
    /// </summary>=
    public static Popup FromToggleButtonResponse(Response buttonResponse)
    {
        SetOpenCommand? command;
        if (buttonResponse.Clicked)
        {
            command = new SetOpenCommand.Toggle();
        }
        else
        {
            command = null;
        }

        return FromResponse(buttonResponse)
            .OpenMemory(command);
    }

    /// <summary>
    /// Show a popup when the widget was clicked.
    /// Sets the layout to <c>Layout.TopDownJustified(Align.Min)</c>.
    /// </summary>
    public static Popup Menu(Response buttonResponse)
    {
        var result = FromToggleButtonResponse(buttonResponse)
            .Kind(PopupKind.Menu)
            .Layout(Egui.Layout.TopDownJustified(Egui.Align.Min))
            .Gap(0);
        result._styleMode = StyleMode.MenuStyle;
        return result;
    }

    /// <summary>
    /// Store the open state via <see cref="Memory"/>.
    /// You can set the state via the first <see cref="SetOpenCommand"/> param.  
    /// </summary>
    public static Popup ContextMenu(Response response)
    {
        SetOpenCommand? command;
        if (response.SecondaryClicked)
        {
            command = new SetOpenCommand.Bool { Value = true };
        }
        else if (response.Clicked)
        {
            command = new SetOpenCommand.Bool { Value = false };
        }
        else
        {
            command = null;
        }

        return Menu(response)
            .OpenMemory(command)
            .AtPointerFixed();
    }

    private Popup(Popup previous, ref bool open)
    {
        this = previous;
        _openKind = OpenKind.Bool;
        _open = ref open;
    }

    /// <summary>
    /// The default ID when constructing a popup from the <see cref="Response"/>. 
    /// </summary>
    public static Id DefaultResponseId(Response response)
    {
        return response.Id.With("popup");
    }

    /// <summary>
    /// Open the given popup and close all others.
    /// 
    /// If you are NOT using <see cref="Show"/>, you must also call
    /// <see cref="Memory.KeepPopupOpen"/> as long as you're showing the popup.  
    /// </summary>
    public static void OpenId(Context ctx, Id id)
    {
        EguiMarshal.Call(EguiFn.egui_containers_popup_Popup_open_id, ctx.Ptr, id);
    }

    /// <summary>
    /// Get the ID of the popup.
    /// </summary>
    public readonly Id GetId()
    {
        return _id;
    }

    /// <summary>
    /// Return the <see cref="PopupAnchor"/> of the popup. 
    /// </summary>
    public readonly PopupAnchor GetAnchor()
    {
        return _anchor;
    }

    /// <summary>
    /// Set the kind of the popup. Used for <see cref="Area.Kind"/> and <see cref="Area.Order"/>.
    /// </summary>
    public readonly Popup Kind(PopupKind kind)
    {
        var result = this;
        result._kind = kind;
        return result;
    }

    /// <summary>
    /// Set the <see cref="UiStackInfo"/> of the popup's <see cref="Ui"/>.  
    /// </summary>
    public readonly Popup Info(UiStackInfo info)
    {
        var result = this;
        result._info = info;
        return result;
    }

    /// <summary>
    /// Set the <see cref="RectAlign"/> of the popup relative to the <see cref="PopupAnchor"/> .
    /// This is the default position, and will be used if it fits.
    /// See <see cref="AlignAlternatives"/> for more on this.
    /// </summary>
    public readonly Popup Align(RectAlign positionAlign)
    {
        var result = this;
        result._rectAlign = positionAlign;
        return result;
    }

    /// <summary>
    /// Set alternative positions to try if the default one doesn't fit. Set to an empty slice to
    /// always use the position you set with <see cref="Align"/> .
    /// By default, this will try <see cref="RectAlign.Symmetries"/> and then <see cref="RectAlign.MenuAligns"/> .
    /// </summary>
    public readonly Popup AlignAlternatives(ImmutableArray<RectAlign> alternatives)
    {
        var result = this;
        result._alternativeAligns = alternatives;
        return result;
    }

    /// <summary>
    /// Force the popup to be open or closed.
    /// </summary>
    public readonly Popup Open(bool open)
    {
        var result = this;
        result._openKind = open ? OpenKind.Open : OpenKind.Closed;
        return result;
    }

    /// <summary>
    /// Store the open state via a mutable bool.
    /// </summary>
    public readonly Popup OpenBool(ref bool open)
    {
        return new Popup(this, ref open);
    }

    public readonly Popup OpenMemory(SetOpenCommand? command)
    {
        var result = this;
        result._openKind = OpenKind.Memory;
        result._memoryCommand = command;
        return result;
    }

    /// <summary>
    /// Set the close behavior of a popup.
    /// 
    /// This will do nothing if <see cref="Open"/> was called. 
    /// </summary>
    public readonly Popup CloseBehavior(PopupCloseBehavior closeBehavior)
    {
        var result = this;
        result._closeBehavior = closeBehavior;
        return result;
    }

    /// <summary>
    /// Show the popup relative to the pointer.
    /// </summary>
    public readonly Popup AtPointer()
    {
        var result = this;
        result._anchor = new PopupAnchor.Pointer();
        return result;
    }

    /// <summary>
    /// Remember the pointer position at the time of opening the popup, and show the popup
    /// relative to that.
    /// </summary>
    public readonly Popup AtPointerFixed()
    {
        var result = this;
        result._anchor = new PopupAnchor.PointerFixed();
        return result;
    }

    /// <summary>
    /// Show the popup relative to a specific position.
    /// </summary>
    public readonly Popup AtPosition(EPos2 position)
    {
        var result = this;
        result._anchor = new PopupAnchor.Position() { Value = position };
        return result;
    }

    /// <summary>
    /// Show the popup relative to the given <see cref="PopupAnchor"/> .
    /// </summary>
    public readonly Popup Anchor(PopupAnchor anchor)
    {
        var result = this;
        result._anchor = anchor;
        return result;
    }

    /// <summary>
    /// Set the gap between the anchor and the popup.
    /// </summary>
    public readonly Popup Gap(float gap)
    {
        var result = this;
        result._gap = gap;
        return result;
    }

    /// <summary>
    /// Set the frame of the popup.
    /// </summary>
    public readonly Popup Frame(Frame frame)
    {
        var result = this;
        result._frame = frame;
        return result;
    }

    /// <summary>
    /// Set the sense of the popup.
    /// </summary>
    public readonly Popup Sense(Sense sense)
    {
        var result = this;
        result._sense = sense;
        return result;
    }

    /// <summary>
    /// Set the layout of the popup.
    /// </summary>
    public readonly Popup Layout(Layout layout)
    {
        var result = this;
        result._layout = layout;
        return result;
    }

    /// <summary>
    /// The width that will be passed to <see cref="Area.DefaultWidth"/> .
    /// </summary>
    public readonly Popup Width(float width)
    {
        var result = this;
        result._width = width;
        return result;
    }

    /// <summary>
    /// Set the id of the Area.
    /// </summary>
    public readonly Popup Id(Id id)
    {
        var result = this;
        result._id = id;
        return result;
    }

    /// <summary>
    /// Set the style for the popup contents.
    /// 
    /// Default: is <c>MenuStyle</c> for <see cref="Menu"/> and <see cref="ContextMenu"/>
    /// or <c>null</c> otherwise.  
    /// </summary>
    public readonly Popup Style(Style style)
    {
        var result = this;
        result._style = style;
        result._styleMode = StyleMode.OverrideStyle;
        return this;
    }

    /// <summary>
    /// Show the popup. Returns None if the popup is not open or anchor is <see cref="PopupAnchor.Pointer"/> and there is no pointer.
    /// </summary>
    public readonly InnerResponse? Show(Action<Ui> addContents)
    {
        var ctx = Ctx;
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var (response, setOpen) = EguiMarshal.Call<nuint, SerializablePopup, bool, EguiCallback, (Response?, bool)>(EguiFn.egui_containers_popup_Popup_show, Ctx.Ptr, new SerializablePopup(this), IsOpen, callback);

        if (_openKind == OpenKind.Bool)
        {
            _open = setOpen;
        }

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
    public readonly InnerResponse<R>? Show<R>(Func<Ui, R> addContents)
    {
        var ctx = Ctx;
        R result = default!;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var (response, setOpen) = EguiMarshal.Call<nuint, SerializablePopup, bool, EguiCallback, (Response?, bool)>(EguiFn.egui_containers_popup_Popup_show, Ctx.Ptr, new SerializablePopup(this), IsOpen, callback);

        if (_openKind == OpenKind.Bool)
        {
            _open = setOpen;
        }

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

    /// <summary>
    /// Display a tooltip popup
    /// </summary>
    internal readonly InnerResponse<R>? ShowTooltip<R>(LayerId parentLayer, Id parentId, Func<Ui, R> addContents)
    {
        var ctx = Ctx;
        R result = default!;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var (response, setOpen) = EguiMarshal.Call<nuint, SerializablePopup, LayerId, Id, EguiCallback, (Response?, bool)>(EguiFn.egui_containers_tooltip_Tooltip_show, Ctx.Ptr, new SerializablePopup(this), parentLayer, parentId, callback);

        if (_openKind == OpenKind.Bool)
        {
            _open = setOpen;
        }

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

    private enum StyleMode
    {
        None,
        MenuStyle,
        OverrideStyle
    }

    private enum OpenKind
    {
        Open,
        Closed,
        Bool,
        Memory
    }

    /// <summary>
    /// Helper struct for serializing popups
    /// </summary>
    private struct SerializablePopup
    {
        public Egui.Id Id;
        public Egui.Containers.PopupAnchor Anchor;
        public Egui.RectAlign RectAlign;
        public ImmutableArray<Egui.RectAlign>? AlternativeAligns;
        public Egui.LayerId LayerId;
        public byte OpenKind;
        public SetOpenCommand? OpenCommand;
        public Egui.Containers.PopupCloseBehavior CloseBehavior;
        public Egui.UiStackInfo? Info;
        public Egui.Containers.PopupKind Kind;
        public float Gap;
        public float? Width;
        public Egui.Sense Sense;
        public Egui.Layout Layout;
        public Egui.Containers.Frame? Frame;
        public Egui.Style? Style;
        public bool MenuStyle;

        public SerializablePopup(Popup popup)
        {
            Id = popup._id;
            Anchor = popup._anchor;
            RectAlign = popup._rectAlign;
            AlternativeAligns = popup._alternativeAligns;
            LayerId = popup._layerId;
            OpenKind = (byte)popup._openKind;
            OpenCommand = popup._memoryCommand;
            CloseBehavior = popup._closeBehavior;
            Info = popup._info;
            Kind = popup._kind;
            Gap = popup._gap;
            Width = popup._width;
            Sense = popup._sense;
            Layout = popup._layout;
            Frame = popup._frame;
            Style = popup._style;
            MenuStyle = popup._styleMode == StyleMode.MenuStyle;
        }

        internal static void Serialize(BincodeSerializer serializer, SerializablePopup value) => value.Serialize(serializer);

        internal void Serialize(BincodeSerializer serializer)
        {
            serializer.increase_container_depth();
            Id.Serialize(serializer);
            Anchor.Serialize(serializer);
            RectAlign.Serialize(serializer);
            serialize_option_vector_RectAlign(AlternativeAligns, serializer);
            LayerId.Serialize(serializer);
            serializer.serialize_u8(OpenKind);
            serialize_option_SetOpenCommmand(OpenCommand, serializer);
            CloseBehavior.Serialize(serializer);
            serialize_option_UiStackInfo(Info, serializer);
            Kind.Serialize(serializer);
            serializer.serialize_f32(Gap);
            Egui.TraitHelpers.serialize_option_f32(Width, serializer);
            Sense.Serialize(serializer);
            Layout.Serialize(serializer);
            Egui.TraitHelpers.serialize_option_Frame(Frame, serializer);
            Egui.TraitHelpers.serialize_option_Style(Style, serializer);
            serializer.serialize_bool(MenuStyle);
            serializer.decrease_container_depth();
        }

        internal static SerializablePopup Deserialize(BincodeDeserializer deserializer)
        {
            throw new NotSupportedException();
        }

        private static void serialize_option_vector_RectAlign(ImmutableArray<Egui.RectAlign>? value, BincodeSerializer serializer)
        {
            if (value is not null)
            {
                serializer.serialize_option_tag(true);
                serialize_vector_RectAlign((value ?? default), serializer);
            }
            else
            {
                serializer.serialize_option_tag(false);
            }
        }

        private static void serialize_option_UiStackInfo(Egui.UiStackInfo? value, BincodeSerializer serializer)
        {
            if (value is not null)
            {
                serializer.serialize_option_tag(true);
                (value ?? default).Serialize(serializer);
            }
            else
            {
                serializer.serialize_option_tag(false);
            }
        }

        private static void serialize_option_SetOpenCommmand(SetOpenCommand? value, BincodeSerializer serializer)
        {
            if (value is not null)
            {
                serializer.serialize_option_tag(true);
                (value ?? default).Serialize(serializer);
            }
            else
            {
                serializer.serialize_option_tag(false);
            }
        }

        private static void serialize_vector_RectAlign(ImmutableArray<Egui.RectAlign> value, BincodeSerializer serializer)
        {
            serializer.serialize_len(value.Length);
            foreach (var item in value)
            {
                item.Serialize(serializer);
            }
        }
    }
}
