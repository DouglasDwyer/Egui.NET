namespace Egui.Util;

/// <summary>
/// The key used in an <see cref="IdTypeMap"/>, which is a combination of an <see cref="Id"/> and
/// a <see cref="Type"/>.<br/>
///
/// This key can be used to remove or access values in the <see cref="IdTypeMap"/> without
/// knowledge of the type that is required for other accessors.
/// </summary>
public sealed class RawKey : IEquatable<RawKey>
{
    private readonly Type _type;
    private readonly Id _id;

    internal Type Type => _type;

    internal Id Id => _id;

    /// <summary>
    /// Create a new key for the given type and id.
    /// </summary>
    public RawKey(Type type, Id id)
    {
        _type = type;
        _id = id;
    }

    /// <inheritdoc/>
    public bool Equals(RawKey? other)
    {
        return other is not null && _type == other._type && _id == other._id;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as RawKey);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(_type, _id);
    }
}
