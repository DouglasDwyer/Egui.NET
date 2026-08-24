using System.ComponentModel;
using System.Numerics;

namespace Egui.Widgets;

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
        _inner.Prefix = new Atoms();
        _inner.Suffix = new Atoms();
        _inner.Speed = 1.0f;
        _inner.Start = double.NegativeInfinity;
        _inner.End = double.PositiveInfinity;
        _inner.ClampExistingToRange = true;
        _inner.MinDecimals = 0;
        _inner.MaxDecimals = null;
        _inner.UpdateWhileEditing = true;

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
    /// will be clamped to the range.<br/>
    ///
    /// ### Without calling <c>Range</c>### With <c>.clampExistingToRange(true)</c> (default)### With <c>.clampExistingToRange(false)</c>    /// </summary>
    public readonly DragValue<T> ClampExistingToRange(bool clampExistingToRange)
    {
        var result = this;
        result._inner.ClampExistingToRange = clampExistingToRange;
        return result;
    }
    /// <summary>
    /// Output the <see cref="DragValue{T}"/>'s <see cref="Egui.Atoms"/>.<br/>
    ///
    /// This includes any images you have on the <see cref="DragValue{T}"/>.
    /// </summary>
    public readonly Atoms Atoms => EguiMarshal.Call<DragValueInner, Atoms>(EguiFn.egui_widgets_drag_value_DragValue_atoms, _inner);

    /// <summary>
    /// Show a prefix before the number, e.g. "x: "
    /// </summary>
    public readonly DragValue<T> Prefix(Atoms prefix)
    {
        var result = this;
        result._inner.Prefix = prefix;
        return result;
    }

    /// <summary>
    /// Add a suffix to the number, this can be e.g. a unit ("°" or " m").
    /// </summary>
    public readonly DragValue<T> Suffix(Atoms suffix)
    {
        var result = this;
        result._inner.Suffix = suffix;
        return result;
    }

    /// <summary>
    /// Set a minimum number of decimals to display.
    /// Normally you don't need to pick a precision, as the slider will intelligently pick a precision for you.
    /// Regardless of precision the slider will use "smart aim" to help the user select nice, round values.
    /// </summary>
    public readonly DragValue<T> MinDecimals(nuint minDecimals)
    {
        var result = this;
        result._inner.MinDecimals = minDecimals;
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
    public readonly DragValue<T> MaxDecimals(nuint maxDecimals)
    {
        var result = this;
        result._inner.MaxDecimals = maxDecimals;
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
        result._inner.MinDecimals = numDecimals;
        result._inner.MaxDecimals = numDecimals;
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
        result._inner.UpdateWhileEditing = update;
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
        public Atoms Prefix;
        public Atoms Suffix;
        public double Start;
        public double End;
        public bool ClampExistingToRange;
        public ulong MinDecimals;
        public ulong? MaxDecimals;
        public bool UpdateWhileEditing;
        public nuint MinWidth;
        public bool TwosComplement;
        public bool Upper;
        public Parser Parser;

        internal static void Serialize(Bincode.BincodeSerializer serializer, DragValueInner value) => value.Serialize(serializer);

        internal void Serialize(Bincode.BincodeSerializer serializer)
        {
            serializer.increase_container_depth();
            serializer.serialize_f64(Speed);
            Prefix.Serialize(serializer);
            Suffix.Serialize(serializer);
            serializer.serialize_f64(Start);
            serializer.serialize_f64(End);
            serializer.serialize_bool(ClampExistingToRange);
            serializer.serialize_u64(MinDecimals);
            Egui.TraitHelpers.serialize_option_u64(MaxDecimals, serializer);
            serializer.serialize_bool(UpdateWhileEditing);
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