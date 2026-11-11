using System.ComponentModel;
using System.Numerics;

namespace Egui;

/// <summary>
/// Select a range of numbers with a two-handled slider.<br/>
///
/// The handles may meet but never cross.
/// Looks and behaves like <see cref="Slider{T}"/>, but with two handles.<br/>
///
/// The default size is set by <see cref="Spacing.SliderWidth"/>.
/// </summary>
public ref struct RangeSlider<T> : IWidget where T : INumber<T>
{
    private ref T _low;
    private ref T _high;
    private SerializableRangeSlider _inner;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("'RangeSlider' does not contain a constructor that takes 0 arguments", error: true)]
    public RangeSlider() { throw new InvalidOperationException(); }

    /// <summary>
    /// Creates a new horizontal range slider.
    ///
    /// The values given will be clamped to the range,
    /// unless you change this behavior with <see cref="Clamping"/> .
    /// </summary>
    public RangeSlider(ref T low, ref T high, T start, T end)
    {
        _low = ref low;
        _high = ref high;

        _inner.Start = double.CreateSaturating(start);
        _inner.End = double.CreateSaturating(end);
        _inner.Spec.Logarithmic = false;
        _inner.Spec.SmallestPositive = 1e-6;
        _inner.Spec.LargestFinite = double.PositiveInfinity;
        _inner.Clamping = SliderClamping.Always;
        _inner.SmartAim = true;
        _inner.DragValue.Show = true;
        _inner.Orientation = SliderOrientation.Horizontal;
        _inner.Text = "";
        _inner.Step = null;
        _inner.DragValue.Speed = null;
        _inner.DragValue.Format.Prefix = new Atoms();
        _inner.DragValue.Format.Suffix = new Atoms();
        _inner.DragValue.Format.UpdateWhileEditing = true;
        _inner.MinSeparation = 0.0;

        if (typeof(T) != typeof(float) && typeof(T) != typeof(double))
        {
            this = Integer();
        }
    }

    /// <summary>
    /// Control whether or not the slider shows the current values.
    /// Default: <c>true</c>.
    /// </summary>
    public readonly RangeSlider<T> ShowValue(bool showValue)
    {
        var result = this;
        result._inner.DragValue.Show = showValue;
        return result;
    }

    /// <summary>
    /// Show a prefix before the number, e.g. "x: "
    /// </summary>
    public readonly RangeSlider<T> Prefix(Atoms prefix)
    {
        var result = this;
        result._inner.DragValue.Format = result._inner.DragValue.Format.WithPrefix(prefix);
        return result;
    }

    /// <summary>
    /// Add a suffix to the number, this can be e.g. a unit ("°" or " m")
    /// </summary>
    public readonly RangeSlider<T> Suffix(Atoms suffix)
    {
        var result = this;
        result._inner.DragValue.Format = result._inner.DragValue.Format.WithSuffix(suffix);
        return result;
    }

    /// <summary>
    /// Show a text next to the slider (e.g. explaining what the slider controls).
    /// </summary>
    public readonly RangeSlider<T> Text(WidgetText text)
    {
        var result = this;
        result._inner.Text = text;
        return result;
    }

    public readonly RangeSlider<T> TextColor(Color32 textColor)
    {
        var result = this;
        result._inner.Text = result._inner.Text.Color(textColor);
        return result;
    }

    /// <summary>
    /// Vertical or horizontal slider? The default is horizontal.
    /// </summary>
    public readonly RangeSlider<T> Orientation(SliderOrientation orientation)
    {
        var result = this;
        result._inner.Orientation = orientation;
        return result;
    }

    /// <summary>
    /// Make this a vertical slider.
    /// </summary>
    public readonly RangeSlider<T> Vertical()
    {
        var result = this;
        result._inner.Orientation = SliderOrientation.Vertical;
        return result;
    }

    /// <summary>
    /// Make this a logarithmic slider.
    /// This is great for when the slider spans a huge range,
    /// e.g. from one to a million.
    /// The default is OFF.
    /// </summary>
    public readonly RangeSlider<T> Logarithmic(bool logarithmic)
    {
        var result = this;
        result._inner.Spec.Logarithmic = logarithmic;
        return result;
    }

    /// <summary>
    /// For logarithmic sliders that includes zero:
    /// what is the smallest positive value you want to be able to select?
    /// The default is `1` for integer sliders and `1e-6` for real sliders.
    /// </summary>
    public readonly RangeSlider<T> SmallestPositive(double smallestPositive)
    {
        var result = this;
        result._inner.Spec.SmallestPositive = smallestPositive;
        return result;
    }

    /// <summary>
    /// For logarithmic sliders, the largest positive value we are interested in
    /// before the slider switches to <see cref="double.PositiveInfinity"/> , if that is the higher end.
    /// Default: <see cref="double.PositiveInfinity"/>.
    /// </summary>
    public readonly RangeSlider<T> LargestFinite(double largestFinite)
    {
        var result = this;
        result._inner.Spec.LargestFinite = largestFinite;
        return result;
    }

    /// <summary>
    /// Controls when the values will be clamped to the range.
    /// </summary>
    public readonly RangeSlider<T> Clamping(SliderClamping clamping)
    {
        var result = this;
        result._inner.Clamping = clamping;
        return result;
    }

    /// <summary>
    /// Turn smart aim on/off. Default is ON.
    /// There is almost no point in turning this off.
    /// </summary>
    public readonly RangeSlider<T> SmartAim(bool smartAim)
    {
        var result = this;
        result._inner.SmartAim = smartAim;
        return result;
    }

    /// <summary>
    /// Sets the minimal change of the value.
    ///
    /// Value <c>0.0</c> effectively disables the feature. If the new value is out of range
    /// and <see cref="ClampToRange"/> is enabled, you would not have the ability to change the value.
    ///
    /// Default: <c>0.0</c> (disabled).
    /// </summary>
    public readonly RangeSlider<T> StepBy(double step)
    {
        var result = this;
        result._inner.Step = step == 0.0 ? null : step;
        return result;
    }

    /// <summary>
    /// When dragging the value, how fast does it move?<br/>
    ///
    /// Unit: values per point (logical pixel).
    /// See also <see cref="DragValue.Speed"/>.<br/>
    ///
    /// By default this is the same speed as when dragging the slider,
    /// but you can change it here to for instance have a much finer control
    /// by dragging the slider value rather than the slider itself.
    /// </summary>
    public readonly RangeSlider<T> DragValueSpeed(double dragValueSpeed)
    {
        var result = this;
        result._inner.DragValue.Speed = dragValueSpeed;
        return result;
    }

    /// <summary>
    /// Set a minimum number of decimals to display.<br/>
    ///
    /// Normally you don't need to pick a precision, as the slider will intelligently pick a precision for you.
    /// Regardless of precision the slider will use "smart aim" to help the user select nice, round values.
    /// </summary>
    public readonly RangeSlider<T> MinDecimals(nuint minDecimals)
    {
        var result = this;
        result._inner.DragValue.Format.MinDecimals = minDecimals;
        return result;
    }

    /// <summary>
    /// Set a maximum number of decimals to display.<br/>
    ///
    /// Values will also be rounded to this number of decimals.
    /// Normally you don't need to pick a precision, as the slider will intelligently pick a precision for you.
    /// Regardless of precision the slider will use "smart aim" to help the user select nice, round values.
    /// </summary>
    public readonly RangeSlider<T> MaxDecimals(nuint maxDecimals)
    {
        var result = this;
        result._inner.DragValue.Format.MaxDecimals = maxDecimals;
        return result;
    }

    public readonly RangeSlider<T> MaxDecimalsOpt(nuint? maxDecimals)
    {
        var result = this;
        result._inner.DragValue.Format.MaxDecimals = maxDecimals;
        return result;
    }

    /// <summary>
    /// Set an exact number of decimals to display.<br/>
    ///
    /// Values will also be rounded to this number of decimals.
    /// Normally you don't need to pick a precision, as the slider will intelligently pick a precision for you.
    /// Regardless of precision the slider will use "smart aim" to help the user select nice, round values.
    /// </summary>
    public readonly RangeSlider<T> FixedDecimals(nuint numDecimals)
    {
        var result = this;
        result._inner.DragValue.Format.MinDecimals = numDecimals;
        result._inner.DragValue.Format.MaxDecimals = numDecimals;
        return result;
    }

    /// <summary>
    /// The closest the handles may come, in values rather than points.<br/>
    ///
    /// Default: <c>0</c>, which lets the range collapse to nothing.
    /// </summary>
    public readonly RangeSlider<T> MinSeparation(T minSeparation)
    {
        var result = this;
        result._inner.MinSeparation = double.Max(double.CreateSaturating(minSeparation), 0.0);
        return result;
    }

    /// <summary>
    /// Change the shape of the slider handle<br/>
    ///
    /// This setting can be enabled globally for all sliders with <see cref="Visuals.HandleShape"/>.
    /// Changing it here will override the above setting ONLY for this individual slider.
    /// </summary>
    public readonly RangeSlider<T> HandleShape(HandleShape handleShape)
    {
        var result = this;
        result._inner.HandleShape = handleShape;
        return result;
    }

    /// <summary>
    /// Replace every setting for the number beside the rail.<br/>
    ///
    /// This also discards any <see cref="Binary"/>, <see cref="Octal"/> or <see cref="Hexadecimal"/> set before.
    /// </summary>
    public readonly RangeSlider<T> DragValue(DragValueSettings dragValue)
    {
        var result = this;
        result._inner.DragValue = dragValue;
        result._inner.Parser = (byte)Parser.Default;
        return result;
    }

    /// <summary>
    /// Replace how the number beside the rail is written and read back.<br/>
    ///
    /// This also discards any <see cref="Binary"/>, <see cref="Octal"/> or <see cref="Hexadecimal"/> set before.
    /// </summary>
    public readonly RangeSlider<T> Format(ValueFormat format)
    {
        var result = this;
        result._inner.DragValue.Format = format;
        result._inner.Parser = (byte)Parser.Default;
        return result;
    }

    /// <summary>
    /// Set how values are spread along the rail.
    /// </summary>
    public readonly RangeSlider<T> Spec(SliderSpec spec)
    {
        var result = this;
        result._inner.Spec = spec;
        return result;
    }

    /// <summary>
    /// Helper: equivalent to <c>this.FixedDecimals(0).SmallestPositive(1).StepBy(1)</c>.
    /// If you use one of the integer constructors this is called for you,
    /// but if you want to have a slider for picking integer values in an <see cref="double"/>, use this.
    /// </summary>
    public readonly RangeSlider<T> Integer()
    {
        return FixedDecimals(0).SmallestPositive(1).StepBy(1);
    }

    /// <summary>
    /// Update the value on each key press when text-editing the value.
    ///
    /// Default: <c>true</c>.
    /// If <c>false</c>, the value will only be updated when user presses enter or deselects the value.
    /// </summary>
    public readonly RangeSlider<T> UpdateWhileEditing(bool updateWhileEditing)
    {
        var result = this;
        result._inner.DragValue.Format.UpdateWhileEditing = updateWhileEditing;
        return result;
    }

    /// <summary>
    /// Set <c>CustomFormatter</c> and <c>CustomParser</c> to display and parse numbers as binary integers. Floating point
    /// numbers are *not* supported.<br/>
    ///
    /// <c>MinWidth</c> specifies the minimum number of displayed digits; if the number is shorter than this, it will be
    /// prefixed with additional 0s to match <c>MinWidth</c>.<br/>
    ///
    /// If <c>TwosComplement</c> is true, negative values will be displayed as the 2's complement representation. Otherwise
    /// they will be prefixed with a '-' sign.<br/>
    ///
    /// # Panics<br/>
    ///
    /// Panics if <c>MinWidth</c> is 0.
    /// </summary>
    public readonly RangeSlider<T> Binary(nuint minWidth, bool twosComplement)
    {
        ArgumentOutOfRangeException.ThrowIfZero(minWidth);
        var result = this;
        result._inner.MinWidth = minWidth;
        result._inner.TwosComplement = twosComplement;
        result._inner.Parser = (byte)Parser.Binary;
        return result;
    }

    /// <summary>
    /// Set <c>CustomFormatter</c> and <c>CustomParser</c> to display and parse numbers as octal integers. Floating point
    /// numbers are *not* supported.<br/>
    ///
    /// <c>MinWidth</c> specifies the minimum number of displayed digits; if the number is shorter than this, it will be
    /// prefixed with additional 0s to match <c>MinWidth</c>.<br/>
    ///
    /// If <c>TwosComplement</c> is true, negative values will be displayed as the 2's complement representation. Otherwise
    /// they will be prefixed with a '-' sign.<br/>
    ///
    /// # Panics<br/>
    ///
    /// Panics if <c>MinWidth</c> is 0.
    /// </summary>
    public readonly RangeSlider<T> Octal(nuint minWidth, bool twosComplement)
    {
        ArgumentOutOfRangeException.ThrowIfZero(minWidth);
        var result = this;
        result._inner.MinWidth = minWidth;
        result._inner.TwosComplement = twosComplement;
        result._inner.Parser = (byte)Parser.Octal;
        return result;
    }

    /// <summary>
    /// Set <c>CustomFormatter</c> and <c>CustomParser</c> to display and parse numbers as hexadecimal integers. Floating point
    /// numbers are *not* supported.<br/>
    ///
    /// <c>MinWidth</c> specifies the minimum number of displayed digits; if the number is shorter than this, it will be
    /// prefixed with additional 0s to match <c>MinWidth</c>.<br/>
    ///
    /// If <c>TwosComplement</c> is true, negative values will be displayed as the 2's complement representation. Otherwise
    /// they will be prefixed with a '-' sign.<br/>
    ///
    /// # Panics<br/>
    ///
    /// Panics if <c>MinWidth</c> is 0.
    /// </summary>
    public readonly RangeSlider<T> Hexadecimal(nuint minWidth, bool twosComplement, bool upper)
    {
        ArgumentOutOfRangeException.ThrowIfZero(minWidth);
        var result = this;
        result._inner.MinWidth = minWidth;
        result._inner.TwosComplement = twosComplement;
        result._inner.Upper = upper;
        result._inner.Parser = (byte)Parser.Hexadecimal;
        return result;
    }

    /// <inheritdoc/>
    Response IWidget.Ui(Ui ui)
    {
        ui.AssertInitialized();
        var low = double.CreateSaturating(_low);
        var high = double.CreateSaturating(_high);
        var (response, newLow, newHigh) = EguiMarshal.Call<nuint, SerializableRangeSlider, double, double, (Response, double, double)>(EguiFn.egui_widgets_range_slider_RangeSlider_ui, ui.Ptr, _inner, low, high);
        _low = T.CreateSaturating(newLow);
        _high = T.CreateSaturating(newHigh);
        return response;
    }

    private partial struct SerializableRangeSlider
    {
        public double Start;
        public double End;
        public SliderSpec Spec;
        public SliderClamping Clamping;
        public bool SmartAim;
        public SliderOrientation Orientation;
        public Egui.WidgetText Text;
        public double? Step;
        public DragValueSettings DragValue;
        public Egui.HandleShape? HandleShape;
        public double MinSeparation;
        public ulong MinWidth;
        public bool TwosComplement;
        public bool Upper;
        public byte Parser;

        internal static void Serialize(Bincode.BincodeSerializer serializer, SerializableRangeSlider value) => value.Serialize(serializer);

        internal void Serialize(Bincode.BincodeSerializer serializer)
        {
            serializer.increase_container_depth();
            serializer.serialize_f64(Start);
            serializer.serialize_f64(End);
            Spec.Serialize(serializer);
            Clamping.Serialize(serializer);
            serializer.serialize_bool(SmartAim);
            Orientation.Serialize(serializer);
            Text.Serialize(serializer);
            Egui.TraitHelpers.serialize_option_f64(Step, serializer);
            DragValue.Serialize(serializer);
            serialize_option_HandleShape(HandleShape, serializer);
            serializer.serialize_f64(MinSeparation);
            serializer.serialize_u64(MinWidth);
            serializer.serialize_bool(TwosComplement);
            serializer.serialize_bool(Upper);
            serializer.serialize_u8(Parser);
            serializer.decrease_container_depth();
        }

        internal static SerializableRangeSlider Deserialize(Bincode.BincodeDeserializer deserializer) => throw new NotSupportedException();

        private static void serialize_option_HandleShape(HandleShape? value, Bincode.BincodeSerializer serializer)
        {
            if (value is not null)
            {
                serializer.serialize_option_tag(true);
                value.Value.Serialize(serializer);
            }
            else
            {
                serializer.serialize_option_tag(false);
            }
        }
    }

    private enum Parser : byte
    {
        Default = 0,
        Binary = 2,
        Octal = 8,
        Hexadecimal = 16
    }
}