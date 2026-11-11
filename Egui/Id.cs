namespace Egui;

/// <summary>
/// egui tracks widgets frame-to-frame using <c>Id</c>s.<br/>
///
/// For instance, if you start dragging a slider one frame, egui stores
/// the sliders <c>Id</c> as the current active id so that next frame when
/// you move the mouse the same slider changes, even if the mouse has
/// moved outside the slider.<br/>
///
/// For some widgets <c>Id</c>s are also used to persist some state about the
/// widgets, such as Window position or whether not a collapsing header region is open.<br/>
///
/// This implies that the <c>Id</c>s must be unique.<br/>
///
/// For simple things like sliders and buttons that don't have any memory and
/// doesn't move we can use the location of the widget as a source of identity.
/// For instance, a slider only needs a unique and persistent ID while you are
/// dragging the slider. As long as it is still while moving, that is fine.<br/>
///
/// For things that need to persist state even after moving (windows, collapsing headers)
/// the location of the widgets is obviously not good enough. For instance,
/// a collapsing region needs to remember whether or not it is open even
/// if the layout next frame is different and the collapsing is not lower down
/// on the screen.<br/>
///
/// Then there are widgets that need no identifiers at all, like labels,
/// because they have no state nor are interacted with.<br/>
///
/// This is niche-optimized to that <c>Option</c> is the same size as <c>Id</c>.
/// </summary>
public partial record struct Id
{
    /// <summary>
    /// The value of the null ID, which is also the ID of a default <see cref="Id"/>.
    /// </summary>
    private const ulong NullValue = ulong.MaxValue;

    /// <summary>
    /// Generate a new <see cref="Id"/> by hashing a string. The ID is globally unique for that string.
    /// </summary>
    public static Id Unique(string source) => EguiMarshal.Call<string, Id>(EguiFn.egui_id_Id_unique, source);

    /// <summary>
    /// Whether this ID holds a value from egui. A default <see cref="Id"/> does not, and is the null ID.
    /// </summary>
    private bool _initialized;

    /// <summary>
    /// The underlying value of the ID.
    /// </summary>
    private ulong _value;

    private readonly ulong Value => _initialized ? _value : NullValue;

    /// <summary>
    /// Generate a new <see cref="Id"/>  by hashing the parent <see cref="Id"/>  and the given salt.
    /// </summary>
    public Id With(IdSalt salt) => WithSalt(salt);

    /// <inheritdoc/>
    public readonly bool Equals(Id other) => Value == other.Value;

    /// <inheritdoc/>
    public override readonly int GetHashCode() => Value.GetHashCode();

    internal static void Serialize(BincodeSerializer serializer, Id value) => value.Serialize(serializer);

    internal readonly void Serialize(BincodeSerializer serializer)
    {
        serializer.increase_container_depth();
        serializer.serialize_u64(Value);
        serializer.decrease_container_depth();
    }

    internal static Id Deserialize(BincodeDeserializer deserializer)
    {
        deserializer.increase_container_depth();
        Id obj = default;
        obj._initialized = true;
        obj._value = deserializer.deserialize_u64();
        deserializer.decrease_container_depth();
        return obj;
    }

    private readonly bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append("Value = ").Append(Value);
        return true;
    }
}
