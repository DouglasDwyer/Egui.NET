using System.Diagnostics.CodeAnalysis;

namespace Egui;

/// <summary>
/// What sort of interaction is a widget sensitive to?
/// </summary>
public readonly partial record struct Sense
{
    /// <summary>
    /// Only hovers detected.
    /// </summary>
    public static readonly Sense Hover = new Sense(0);

    /// <summary>
    /// Buttons, sliders, windows, ...
    /// </summary>
    public static readonly Sense ClickNoFocus = new Sense(1 << 0);

    /// <summary>
    /// Sliders, windows, scroll bars, scroll areas, ...
    /// </summary>
    public static readonly Sense DragNoFocus = new Sense(1 << 1);

    /// <summary>
    /// This widget wants focus.
    ///
    /// Anything interactive + labels that can be focused
    /// for the benefit of screen readers.
    /// </summary>
    public static readonly Sense Focusable = new Sense(1 << 2);

    /// <summary>
    /// Sense drags and hover, but not clicks.
    /// </summary>
    public static readonly Sense Click = ClickNoFocus | Focusable;

    /// <summary>
    /// Sense drags and hover, but not clicks.
    /// </summary>
    public static readonly Sense Drag = DragNoFocus | Focusable;

    /// <summary>
    /// Sense both clicks, drags and hover (e.g. a slider or window).<br/>
    /// Note that this will introduce a latency when dragging, because when the user starts a press egui can't know if this is the start of a click or a drag, and it won't know until the cursor has either moved a certain distance, or the user has released the mouse button.<br/>
    /// See <c>IsDecidedlyDragging</c> for details.
    /// </summary>
    public static readonly Sense ClickAndDrag = ClickNoFocus | DragNoFocus | Focusable;

    /// <summary>
    /// The internal value of the enum.
    /// </summary>
    private readonly byte _value;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="value">The underlying enum value to assign.</param>
    private Sense(byte value)
    {
        _value = value;
    }

    internal static void Serialize(BincodeSerializer serializer, Sense sense) => sense.Serialize(serializer);

    internal void Serialize(BincodeSerializer serializer)
    {
        serializer.increase_container_depth();
        serializer.serialize_u8(_value);
        serializer.decrease_container_depth();
    }

    internal static Sense Deserialize(BincodeDeserializer deserializer) {
        deserializer.increase_container_depth();
        Sense value = (Sense)deserializer.deserialize_u8();
        deserializer.decrease_container_depth();
        return value;
    }

    /// <summary>
    /// Inverts the current sense flags.
    /// </summary>
    /// <param name="flags">The set of flags.</param>
    /// <returns>The new sense value.</returns>
    public static Sense operator ~(Sense flags) => (Sense)~flags._value;

    /// <summary>
    /// Gets the underlying bitwise representation of the flags.
    /// </summary>
    /// <param name="flags">The flags in use.</param>
    public static explicit operator byte(Sense flags) => flags._value;

    /// <summary>
    /// Converts from a raw bitwise representation to a set of flags.
    /// </summary>
    /// <param name="flags">The underlying representation of the flags.</param>
    public static explicit operator Sense(byte flags) => new Sense((byte)(flags & 0b111));

    /// <summary>
    /// Takes the setwise <c>and</c> of the sense flags.
    /// </summary>
    /// <param name="lhs">The first set of flags.</param>
    /// <param name="rhs">The second set of flags.</param>
    /// <returns>The new sense value.</returns>
    public static Sense operator &(Sense lhs, Sense rhs) => new Sense((byte)(lhs._value | rhs._value));

    /// <summary>
    /// Takes the setwise <c>or</c> of the sense flags.
    /// </summary>
    /// <param name="lhs">The first set of flags.</param>
    /// <param name="rhs">The second set of flags.</param>
    /// <returns>The new sense value.</returns>
    public static Sense operator |(Sense lhs, Sense rhs) => new Sense((byte)(lhs._value | rhs._value));

    /// <summary>
    /// Restricts the compiler-synthesized <see cref="ToString"/> to just the underlying
    /// flags value, rather than every convenience property defined on this type.
    /// </summary>
    private readonly bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append("Value = ").Append(_value);
        return true;
    }
}