using System.ComponentModel;
using System.Numerics;

namespace Egui;

/// <summary>
/// A numeric value that you can change by dragging the number. More compact than a <see cref="Slider"/>.
/// </summary>
public ref struct DragValue<T> : IWidget where T : INumber<T>
{
    private ref T _value;
    private DragValueInner _inner;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("'DragValue' does not contain a constructor that takes 0 arguments", error: true)]
    public DragValue() { throw new InvalidOperationException(); }

    public DragValue(ref T value)
    {
        _value = ref value;
        _inner.Speed = 1.0;
        _inner.Start = double.NegativeInfinity;
        _inner.End = double.PositiveInfinity;
        _inner.ClampExistingToRange = true;
        _inner.Format.Prefix = new Atoms();
        _inner.Format.Suffix = new Atoms();
        _inner.Format.UpdateWhileEditing = true;
        _inner.Classes = new Classes();

        if (typeof(T) != typeof(float) && typeof(T) != typeof(double))
        {
            this = MaxDecimals(0)
                .Range(T.CreateSaturating(double.MinValue), T.CreateSaturating(double.MaxValue))
                .Speed(0.25);
        }
    }

    /// <summary>
    /// How much the value changes when dragged one point (logical pixel).<br/>
    ///
    /// Should be finite and greater than zero.
    /// </summary>
    public readonly DragValue<T> Speed(double speed)
    {
        var result = this;
        result._inner.Speed = speed;
        return result;
    }

    /// <summary>
    /// Sets valid range for dragging the value.<br/>
    ///
    /// By default all values are clamped to this range, even when not interacted with.
    /// You can change this behavior by passing <c>False</c> to <c>ClampExistingToRange</c>.
    /// </summary>
    public readonly DragValue<T> Range(T start, T end)
    {
        var result = this;
        result._inner.Start = double.CreateSaturating(start);
        result._inner.End = double.CreateSaturating(end);
        return result;
    }

    /// <summary>
    /// If set to <c>True</c>, existing values will be clamped to <c>Range</c>.<br/>
    ///
    /// If <c>False</c>, only values entered by the user (via dragging or text editing)
    /// will be clamped to the range.
    /// </summary>
    public readonly DragValue<T> ClampExistingToRange(bool clampExistingToRange)
    {
        var result = this;
        result._inner.ClampExistingToRange = clampExistingToRange;
        return result;
    }
    /// <summary>
    /// Replace how the number is written and read back.<br/>
    ///
    /// This also discards any <see cref="Binary"/>, <see cref="Octal"/> or <see cref="Hexadecimal"/> set before.
    /// </summary>
    public readonly DragValue<T> Format(ValueFormat format)
    {
        var result = this;
        result._inner.Format = format;
        result._inner.Parser = Parser.Default;
        return result;
    }

    /// <summary>
    /// The minimum size of the drag value, in both its dragged and its text-edited state.<br/>
    ///
    /// Defaults to <see cref="Spacing.InteractSize"/>.
    /// </summary>
    public readonly DragValue<T> MinSize(EVec2 minSize)
    {
        var result = this;
        result._inner.MinSize = minSize;
        return result;
    }

    /// <summary>
    /// Keep the value inside the widget's width when text-editing it.<br/>
    ///
    /// Default: <c>False</c>, which lets the widget widen to fit whatever has been typed. When <c>True</c>
    /// the widget holds its width and the text scrolls inside it, following the cursor.
    /// </summary>
    public readonly DragValue<T> ClipText(bool clipText)
    {
        var result = this;
        result._inner.ClipText = clipText;
        return result;
    }

    /// <summary>
    /// Append the given classes to the drag value, deduplicating them.
    /// </summary>
    public readonly DragValue<T> Classes(Classes classes)
    {
        var result = this;
        result._inner.Classes = classes;
        return result;
    }

    /// <summary>
    /// Output the <see cref="DragValue{T}"/>'s <see cref="Egui.Atoms"/>.<br/>
    ///
    /// This includes any images you have on the <see cref="DragValue{T}"/>.
    /// </summary>
    public readonly Atoms Atoms => EguiMarshal.Call<DragValueInner, Atoms>(EguiFn.egui_widgets_drag_value_DragValue_atoms, _inner);

    /// <summary>
    /// Show a prefix before the number, e.g. "x: ".<br/>
    ///
    /// Goes in front of any prefix already set, so <c>.Prefix("b").Prefix("a")</c> shows <c>ab</c>.
    /// </summary>
    public readonly DragValue<T> Prefix(Atoms prefix)
    {
        var result = this;
        result._inner.Format = result._inner.Format.WithPrefix(prefix);
        return result;
    }

    /// <summary>
    /// Add a suffix to the number, this can be e.g. a unit ("°" or " m").<br/>
    ///
    /// Goes after any suffix already set, so <c>.Suffix("a").Suffix("b")</c> shows <c>ab</c>.
    /// </summary>
    public readonly DragValue<T> Suffix(Atoms suffix)
    {
        var result = this;
        result._inner.Format = result._inner.Format.WithSuffix(suffix);
        return result;
    }

    /// <summary>
    /// Set a minimum number of decimals to display.
    /// </summary>
    public readonly DragValue<T> MinDecimals(nuint minDecimals)
    {
        var result = this;
        result._inner.Format.MinDecimals = minDecimals;
        return result;
    }

    /// <summary>
    /// Set a maximum number of decimals to display.
    /// Values will also be rounded to this number of decimals.
    /// </summary>
    public readonly DragValue<T> MaxDecimals(nuint maxDecimals)
    {
        var result = this;
        result._inner.Format.MaxDecimals = maxDecimals;
        return result;
    }

    /// <summary>
    /// Set an exact number of decimals to display.
    /// Values will also be rounded to this number of decimals.
    /// Normally you don't need to pick a precision, as the slider will intelligently pick a precision for you.
    /// Regardless of precision the slider will use "smart aim" to help the user select nice, round values.
    /// </summary>
    public readonly DragValue<T> FixedDecimals(nuint numDecimals)
    {
        var result = this;
        result._inner.Format.MinDecimals = numDecimals;
        result._inner.Format.MaxDecimals = numDecimals;
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
    public readonly DragValue<T> Binary(nuint minWidth, bool twosComplement)
    {
        ArgumentOutOfRangeException.ThrowIfZero(minWidth);
        var result = this;
        result._inner.MinWidth = minWidth;
        result._inner.TwosComplement = twosComplement;
        result._inner.Parser = Parser.Binary;
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
    public readonly DragValue<T> Octal(nuint minWidth, bool twosComplement)
    {
        ArgumentOutOfRangeException.ThrowIfZero(minWidth);
        var result = this;
        result._inner.MinWidth = minWidth;
        result._inner.TwosComplement = twosComplement;
        result._inner.Parser = Parser.Octal;
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
    public readonly DragValue<T> Hexadecimal(nuint minWidth, bool twosComplement, bool upper)
    {
        ArgumentOutOfRangeException.ThrowIfZero(minWidth);
        var result = this;
        result._inner.MinWidth = minWidth;
        result._inner.TwosComplement = twosComplement;
        result._inner.Upper = upper;
        result._inner.Parser = Parser.Hexadecimal;
        return result;
    }

    /// <summary>
    /// Update the value on each key press when text-editing the value.<br/>
    ///
    /// Default: <c>True</c>.
    /// If <c>False</c>, the value will only be updated when user presses enter or deselects the value.
    /// </summary>
    public readonly DragValue<T> UpdateWhileEditing(bool update)
    {
        var result = this;
        result._inner.Format.UpdateWhileEditing = update;
        return result;
    }

    /// <inheritdoc/>
    Response IWidget.Ui(Ui ui)
    {
        ui.AssertInitialized();
        var value = double.CreateSaturating(_value);
        var (response, newValue) = EguiMarshal.Call<nuint, DragValueInner, double, (Response, double)>(EguiFn.egui_widgets_drag_value_DragValue_ui, ui.Ptr, _inner, value);
        _value = T.CreateSaturating(newValue);
        return response;
    }

    private partial struct DragValueInner
    {
        public double Speed;
        public double Start;
        public double End;
        public bool ClampExistingToRange;
        public ValueFormat Format;
        public Classes Classes;
        public EVec2? MinSize;
        public bool ClipText;
        public nuint MinWidth;
        public bool TwosComplement;
        public bool Upper;
        public Parser Parser;

        internal static void Serialize(Bincode.BincodeSerializer serializer, DragValueInner value) => value.Serialize(serializer);

        internal void Serialize(Bincode.BincodeSerializer serializer)
        {
            serializer.increase_container_depth();
            serializer.serialize_f64(Speed);
            serializer.serialize_f64(Start);
            serializer.serialize_f64(End);
            serializer.serialize_bool(ClampExistingToRange);
            Format.Serialize(serializer);
            Classes.Serialize(serializer);
            Egui.TraitHelpers.serialize_option_EVec2(MinSize, serializer);
            serializer.serialize_bool(ClipText);
            serializer.serialize_u64((ulong)MinWidth);
            serializer.serialize_bool(TwosComplement);
            serializer.serialize_bool(Upper);
            serializer.serialize_u8((byte)Parser);
            serializer.decrease_container_depth();
        }

        internal static DragValueInner Deserialize(Bincode.BincodeDeserializer deserializer) => throw new NotImplementedException();
    }

    private enum Parser : byte
    {
        Default = 0,
        Binary = 2,
        Octal = 8,
        Hexadecimal = 16
    }
}