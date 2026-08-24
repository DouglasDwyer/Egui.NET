using System.Diagnostics.CodeAnalysis;

namespace Egui.Containers;

/// <summary>
/// Specifies which pointer buttons can be used to pan the scene by dragging.
/// </summary>
public readonly partial record struct DragPanButtons
{
    /// <summary>
    /// No flags.
    /// </summary>
    public static readonly DragPanButtons None = new DragPanButtons(0);

    /// <summary>
    /// <see cref="PointerButton.Primary"/> 
    /// </summary>
    public static readonly DragPanButtons Primary = new DragPanButtons(1 << 0);

    /// <summary>
    /// <see cref="PointerButton.Secondary"/> 
    /// </summary>
    public static readonly DragPanButtons Secondary = new DragPanButtons(1 << 1);

    /// <summary>
    /// <see cref="PointerButton.Middle"/> 
    /// </summary>
    public static readonly DragPanButtons Middle = new DragPanButtons(1 << 2);

    /// <summary>
    /// <see cref="PointerButton.Extra1"/> 
    /// </summary>
    public static readonly DragPanButtons Extra1 = new DragPanButtons(1 << 3);

    /// <summary>
    /// <see cref="PointerButton.Extra2"/> 
    /// </summary>
    public static readonly DragPanButtons Extra2 = new DragPanButtons(1 << 4);

    /// <summary>
    /// All flags.
    /// </summary>
    public static readonly DragPanButtons All = Primary | Secondary | Middle | Extra1 | Extra2;

    /// <summary>
    /// The internal value of the enum.
    /// </summary>
    private readonly byte _value;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="value">The underlying enum value to assign.</param>
    private DragPanButtons(byte value)
    {
        _value = value;
    }

    internal void Serialize(BincodeSerializer serializer)
    {
        serializer.increase_container_depth();
        serializer.serialize_variant_index(_value);
        serializer.decrease_container_depth();
    }

    internal static DragPanButtons Deserialize(BincodeDeserializer deserializer)
    {
        deserializer.increase_container_depth();
        int index = deserializer.deserialize_variant_index();
        if (!Enum.IsDefined(typeof(DragPanButtons), index))
            throw new Serde.DeserializationException("Unknown variant index for DragPanButtons: " + index);
        DragPanButtons value = (DragPanButtons)index;
        deserializer.decrease_container_depth();
        return value;
    }

    /// <summary>
    /// Inverts the current set of flags.
    /// </summary>
    /// <param name="flags">The set of flags.</param>
    /// <returns>The new flags value.</returns>
    public static DragPanButtons operator ~(DragPanButtons flags) => (DragPanButtons)~flags._value;

    /// <summary>
    /// Gets the underlying bitwise representation of the flags.
    /// </summary>
    /// <param name="flags">The flags in use.</param>
    public static explicit operator byte(DragPanButtons flags) => flags._value;

    /// <summary>
    /// Converts from a raw bitwise representation to a set of flags.
    /// </summary>
    /// <param name="flags">The underlying representation of the flags.</param>
    public static explicit operator DragPanButtons(byte flags) => new DragPanButtons((byte)(flags & 0b11111));

    /// <summary>
    /// Takes the setwise <c>and</c> of the flags.
    /// </summary>
    /// <param name="lhs">The first set of flags.</param>
    /// <param name="rhs">The second set of flags.</param>
    /// <returns>The new flags value.</returns>
    public static DragPanButtons operator &(DragPanButtons lhs, DragPanButtons rhs) => new DragPanButtons((byte)(lhs._value | rhs._value));

    /// <summary>
    /// Takes the setwise <c>or</c> of the flags.
    /// </summary>
    /// <param name="lhs">The first set of flags.</param>
    /// <param name="rhs">The second set of flags.</param>
    /// <returns>The new flags value.</returns>
    public static DragPanButtons operator |(DragPanButtons lhs, DragPanButtons rhs) => new DragPanButtons((byte)(lhs._value | rhs._value));

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