using System.ComponentModel;
using System.Numerics;

namespace Egui.Widgets;

/// <summary>
/// Control a number with a slider.<br/>
///
/// The slider range defines the values you get when pulling the slider to the far edges.
/// By default all values are clamped to this range, even when not interacted with.
/// You can change this behavior by passing <see cref="SliderClamping.Never"/> to <see cref="Clamping"/>.<br/>
///
/// The range can include any numbers, and go from low-to-high or from high-to-low.<br/>
///
/// The slider consists of three parts: a slider, a value display, and an optional text.
/// The user can click the value display to edit its value. It can be turned off with <see cref="ShowValue"/>.
/// </summary>
public ref struct Slider<T> : IWidget where T : INumber<T>
{
    private ref T _value;
    private SerializableSlider _inner;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("'Slider' does not contain a constructor that takes 0 arguments", error: true)]
    public Slider() { throw new InvalidOperationException(); }

    /// <summary>
    /// Creates a new horizontal slider.
    ///
    /// The <paramref name="value"/> given will be clamped to the range,
    /// unless you change this behavior with <see cref="Clamping"/> .
    /// </summary>
    public Slider(ref T value, T start, T end)
    {
        _value = ref value;

        _inner.Start = double.CreateSaturating(start);
        _inner.End = double.CreateSaturating(end);
        _inner.Spec = new SliderSpec
        {
            Logarithmic = false,
            SmallestPositive = 1e-6,
            LargestFinite = double.PositiveInfinity
        };
        _inner.Clamping = SliderClamping.Always;
        _inner.SmartAim = true;
        _inner.ShowValue = true;
        _inner.Orientation = SliderOrientation.Horizontal;
        _inner.Prefix = "";
        _inner.Suffix = "";
        _inner.Text = "";
        _inner.Step = null;
        _inner.DragValueSpeed = null;
        _inner.MinDecimals = 0;
        _inner.TrailingFill = null;
        _inner.UpdateWhileEditing = true;

        if (typeof(T) != typeof(float) && typeof(T) != typeof(double))
        {
            this = Integer();
        }
    }

    /// <summary>
    /// Control whether or not the slider shows the current value.
    /// Default: <c>true</c>.
    /// </summary>
    public readonly Slider<T> ShowValue(bool showValue)
    {
        var result = this;
        result._inner.ShowValue = showValue;
        return result;
    }

    /// <summary>
    /// Show a prefix before the number, e.g. "x: "
    /// </summary>
    public readonly Slider<T> Prefix(string prefix)
    {
        var result = this;
        result._inner.Prefix = prefix;
        return result;
    }

    /// <summary>
    /// Add a suffix to the number, this can be e.g. a unit ("°" or " m")
    /// </summary>
    public readonly Slider<T> Suffix(string suffix)
    {
        var result = this;
        result._inner.Suffix = suffix;
        return result;
    }

    /// <summary>
    /// Show a text next to the slider (e.g. explaining what the slider controls).
    /// </summary>
    public readonly Slider<T> Text(string text)
    {
        var result = this;
        result._inner.Text = text;
        return result;
    }

    public readonly Slider<T> TextColor(Color32 textColor)
    {
        var result = this;
        result._inner.Text = result._inner.Text.Color(textColor);
        return result;
    }

    /// <summary>
    /// Vertical or horizontal slider? The default is horizontal.
    /// </summary>
    public readonly Slider<T> Orientation(SliderOrientation orientation)
    {
        var result = this;
        result._inner.Orientation = orientation;
        return result;
    }

    /// <summary>
    /// Make this a vertical slider.
    /// </summary>
    public readonly Slider<T> Vertical()
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
    public readonly Slider<T> Logarithmic(bool logarithmic)
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
    public readonly Slider<T> SmallestPositive(double smallestPositive)
    {
        var result = this;
        result._inner.Spec.SmallestPositive = smallestPositive;
        return result;
    }

    /// <summary>
    /// For logarithmic sliders, the largest positive value we are interested in
    /// before the slider switches to <see cref="double.PositiveInfinity"/> , if that is the higher end.
    /// Default: <see cref="double.PositiveInfinity">.
    /// </summary>
    public readonly Slider<T> LargestFinite(double smallestPositive)
    {
        var result = this;
        result._inner.Spec.SmallestPositive = smallestPositive;
        return result;
    }

    /// <summary>
    /// Controls when the values will be clamped to the range.
    /// </summary>
    public readonly Slider<T> Clamping(SliderClamping clamping)
    {
        var result = this;
        result._inner.Clamping = clamping;
        return result;
    }

    /// <summary>
    /// Turn smart aim on/off. Default is ON.
    /// There is almost no point in turning this off.
    /// </summary>
    public readonly Slider<T> SmartAim(bool smartAim)
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
    public readonly Slider<T> StepBy(double step)
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
    public readonly Slider<T> DragValueSpeed(double dragValueSpeed)
    {
        var result = this;
        result._inner.DragValueSpeed = dragValueSpeed;
        return result;
    }

    /// <summary>
    /// Set a minimum number of decimals to display.<br/>
    ///
    /// Normally you don't need to pick a precision, as the slider will intelligently pick a precision for you.
    /// Regardless of precision the slider will use "smart aim" to help the user select nice, round values.
    /// </summary>
    public readonly Slider<T> MinDecimals(nuint minDecimals)
    {
        var result = this;
        result._inner.MinDecimals = minDecimals;
        return result;
    }

    /// <summary>
    /// Set a maximum number of decimals to display.<br/>
    ///
    /// Values will also be rounded to this number of decimals.
    /// Normally you don't need to pick a precision, as the slider will intelligently pick a precision for you.
    /// Regardless of precision the slider will use "smart aim" to help the user select nice, round values.
    /// </summary>
    public readonly Slider<T> MaxDecimals(nuint maxDecimals)
    {
        var result = this;
        result._inner.MaxDecimals = maxDecimals;
        return result;
    }

    public readonly Slider<T> MaxDecimalsOpt(nuint? maxDecimals)
    {
        var result = this;
        result._inner.MaxDecimals = maxDecimals;
        return result;
    }

    /// <summary>
    /// Set an exact number of decimals to display.<br/>
    ///
    /// Values will also be rounded to this number of decimals.
    /// Normally you don't need to pick a precision, as the slider will intelligently pick a precision for you.
    /// Regardless of precision the slider will use "smart aim" to help the user select nice, round values.
    /// </summary>
    public readonly Slider<T> FixedDecimals(nuint numDecimals)
    {
        var result = this;
        result._inner.MinDecimals = numDecimals;
        result._inner.MaxDecimals = numDecimals;
        return result;
    }

    /// <summary>
    /// Display trailing color behind the slider's circle. Default is OFF.
    ///
    /// This setting can be enabled globally for all sliders with <see cref="Visuals.SliderTrailingFill">.
    /// Toggling it here will override the above setting ONLY for this individual slider.
    ///
    /// The fill color will be taken from <c>Selection.BgFill</c> in your <see cref="Visuals"/>, the same as a <see cref="ProgressBar"/>. 
    /// </summary>
    public readonly Slider<T> TrailingFill(bool trailingFill)
    {
        var result = this;
        result._inner.TrailingFill = trailingFill;
        return result;
    }

    /// <summary>
    /// Change the shape of the slider handle<br/>
    ///
    /// This setting can be enabled globally for all sliders with <see cref="Visuals.HandleShape"/>.
    /// Changing it here will override the above setting ONLY for this individual slider.
    /// </summary>
    public readonly Slider<T> HandleShape(HandleShape handleShape)
    {
        var result = this;
        result._inner.HandleShape = handleShape;
        return result;
    }

    /// <summary>
    /// Helper: equivalent to <c>this.FixedDecimals(0).SmallestPositive(1).StepBy(1)</c>.
    /// If you use one of the integer constructors this is called for you,
    /// but if you want to have a slider for picking integer values in an <see cref="double"/>, use this.
    /// </summary>
    public readonly Slider<T> Integer()
    {
        return FixedDecimals(0).SmallestPositive(1).StepBy(1);
    }

    /// <summary>
    /// Update the value on each key press when text-editing the value.
    ///
    /// Default: <c>true</c>.
    /// If <c>false</c>, the value will only be updated when user presses enter or deselects the value.
    /// </summary>
    public readonly Slider<T> UpdateWhileEditing(bool updateWhileEditing)
    {
        var result = this;
        result._inner.UpdateWhileEditing = updateWhileEditing;
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
    public readonly Slider<T> Binary(nuint minWidth, bool twosComplement)
    {
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
    public readonly Slider<T> Octal(nuint minWidth, bool twosComplement)
    {
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
    public readonly Slider<T> Hexadecimal(nuint minWidth, bool twosComplement, bool upper)
    {
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
        var value = double.CreateSaturating(_value);
        var (response, newValue) = EguiMarshal.Call<nuint, SerializableSlider, double, (Response, double)>(EguiFn.egui_widgets_slider_Slider_ui, ui.Ptr, _inner, value);
        _value = T.CreateSaturating(newValue);
        return response;
    }

    private struct SliderSpec
    {
        public bool Logarithmic;
        public double SmallestPositive;
        public double LargestFinite;


        internal static void Serialize(Bincode.BincodeSerializer serializer, SliderSpec value) => value.Serialize(serializer);

        internal void Serialize(Bincode.BincodeSerializer serializer)
        {
            serializer.increase_container_depth();
            serializer.serialize_bool(Logarithmic);
            serializer.serialize_f64(SmallestPositive);
            serializer.serialize_f64(LargestFinite);
            serializer.decrease_container_depth();
        }

        internal static SliderSpec Deserialize(Bincode.BincodeDeserializer deserializer) => throw new NotSupportedException();
    }

    private partial struct SerializableSlider
    {
        public double Start;
        public double End;
        public SliderSpec Spec;
        public SliderClamping Clamping;
        public bool SmartAim;
        public bool ShowValue;
        public SliderOrientation Orientation;
        public string Prefix;
        public string Suffix;
        public Egui.WidgetText Text;
        public double? Step;
        public double? DragValueSpeed;
        public ulong MinDecimals;
        public ulong? MaxDecimals;
        public bool? TrailingFill;
        public Egui.HandleShape? HandleShape;
        public bool UpdateWhileEditing;
        public ulong MinWidth;
        public bool TwosComplement;
        public bool Upper;
        public byte Parser;

        internal static void Serialize(Bincode.BincodeSerializer serializer, SerializableSlider value) => value.Serialize(serializer);

        internal void Serialize(Bincode.BincodeSerializer serializer)
        {
            serializer.increase_container_depth();
            serializer.serialize_f64(Start);
            serializer.serialize_f64(End);
            Spec.Serialize(serializer);
            Clamping.Serialize(serializer);
            serializer.serialize_bool(SmartAim);
            serializer.serialize_bool(ShowValue);
            Orientation.Serialize(serializer);
            serializer.serialize_str(Prefix);
            serializer.serialize_str(Suffix);
            Text.Serialize(serializer);
            Egui.TraitHelpers.serialize_option_f64(Step, serializer);
            Egui.TraitHelpers.serialize_option_f64(DragValueSpeed, serializer);
            serializer.serialize_u64(MinDecimals);
            Egui.TraitHelpers.serialize_option_u64(MaxDecimals, serializer);
            Egui.TraitHelpers.serialize_option_bool(TrailingFill, serializer);
            serialize_option_HandleShape(HandleShape, serializer);
            serializer.serialize_bool(UpdateWhileEditing);
            serializer.serialize_u64(MinWidth);
            serializer.serialize_bool(TwosComplement);
            serializer.serialize_bool(Upper);
            serializer.serialize_u8(Parser);
            serializer.decrease_container_depth();
        }

        internal static SerializableSlider Deserialize(Bincode.BincodeDeserializer deserializer) => throw new NotSupportedException();

        private static void serialize_option_HandleShape(HandleShape? value, Bincode.BincodeSerializer serializer) {
            if (value is not null) {
                serializer.serialize_option_tag(true);
                value.Value.Serialize(serializer);
            } else {
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