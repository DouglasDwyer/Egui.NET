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
    /// A special <see cref="Id"/> , in particular as a key to <see cref="Memory.Data"/> 
    /// for when there is no particular widget to attach the data.
    ///
    /// The null <see cref="Id"/>  is still a valid id to use in all circumstances,
    /// though obviously it will lead to a lot of collisions if you do use it!
    /// </summary>
    public static readonly Id Null = new Id { _value = ulong.MaxValue };

    /// <summary>
    /// The underlying value of the ID.
    /// </summary>
    private ulong _value;

    /// Generate a new <see cref="Id"/>  by hashing some source.
    public Id(string source)
    {
        this = EguiMarshal.Call<string, Id>(EguiFn.egui_id_Id_new, source);
    }

    /// <summary>
    /// Generate a new <see cref="Id"/>  by hashing the parent <see cref="Id"/>  and the given argument.
    /// </summary>
    public readonly Id With(string child)
    {
        return EguiMarshal.Call<Id, string, Id>(EguiFn.egui_id_Id_with, this, child);
    }

    /// <summary>
    /// Converts a string to an ID.
    /// </summary>
    /// <param name="source">The source to use.</param>
    public static implicit operator Id(string source) => new Id(source);

    internal static void Serialize(BincodeSerializer serializer, Id value) => value.Serialize(serializer);

    internal void Serialize(BincodeSerializer serializer)
    {
        serializer.increase_container_depth();
        serializer.serialize_u64(_value);
        serializer.decrease_container_depth();
    }

    internal static Id Deserialize(BincodeDeserializer deserializer)
    {
        deserializer.increase_container_depth();
        Id obj = default;
        obj._value = deserializer.deserialize_u64();
        deserializer.decrease_container_depth();
        return obj;
    }

    /// <summary>
    /// Restricts the compiler-synthesized <see cref="ToString"/> to just the underlying ID
    /// value, rather than every convenience property defined on this type.
    /// </summary>
    private readonly bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append("Value = ").Append(_value);
        return true;
    }
}