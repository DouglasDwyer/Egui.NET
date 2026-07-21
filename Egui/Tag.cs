using System.Text;

namespace Egui;

/// <summary>
/// An OpenType tag, e.g. <c>"wght"</c> for the weight variation axis.<br/>
///
/// This mirrors Rust's <c>font_types::Tag</c>, which egui's <c>IntoTag</c> functions
/// (e.g. <see cref="RichText.Variation"/>) accept. <c>Tag</c> isn't a type that egui itself
/// exposes to C# through the usual reflection-based bindings (it lives in an external crate,
/// re-exported but never traced), so it's hand-written here, with implicit conversions from
/// every type that implements <c>IntoTag</c> in Rust (<c>string</c>, <c>int</c>, <c>uint</c>,
/// and raw 4-byte arrays) so callers can pass any of them directly.
/// </summary>
public partial struct Tag : IEquatable<Tag>
{
    private Array4<byte> _value;

    /// <summary>
    /// Creates a tag from its 4 raw bytes.
    /// </summary>
    public Tag(byte b0, byte b1, byte b2, byte b3)
    {
        _value = new Array4<byte>(b0, b1, b2, b3);
    }

    /// <summary>
    /// Creates a tag from a 1-4 character ASCII string, padding with spaces if shorter.
    /// </summary>
    public static implicit operator Tag(string value)
    {
        if (value.Length == 0 || value.Length > 4)
        {
            throw new ArgumentException("A tag must be between 1 and 4 characters long.", nameof(value));
        }

        var bytes = Encoding.ASCII.GetBytes(value);
        return new Tag(
            bytes[0],
            bytes.Length > 1 ? bytes[1] : (byte)' ',
            bytes.Length > 2 ? bytes[2] : (byte)' ',
            bytes.Length > 3 ? bytes[3] : (byte)' ');
    }

    /// <summary>
    /// Decodes the tag back into an ASCII string, trimming any padding spaces.
    /// </summary>
    public static implicit operator string(Tag tag)
    {
        return Encoding.ASCII.GetString([tag._value[0], tag._value[1], tag._value[2], tag._value[3]]).TrimEnd(' ');
    }

    /// <summary>
    /// Creates a tag from a big-endian-packed <c>uint</c> (e.g. <c>0x77676874</c> for <c>"wght"</c>).
    /// </summary>
    public static implicit operator Tag(uint value)
    {
        return new Tag((byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value);
    }

    /// <summary>
    /// Packs the tag into a big-endian <c>uint</c>.
    /// </summary>
    public static implicit operator uint(Tag tag)
    {
        return ((uint)tag._value[0] << 24) | ((uint)tag._value[1] << 16) | ((uint)tag._value[2] << 8) | tag._value[3];
    }

    /// <summary>
    /// Creates a tag from a big-endian-packed <c>int</c>, matching the <c>uint</c> conversion.
    /// </summary>
    public static implicit operator Tag(int value) => (uint)value;

    /// <summary>
    /// Packs the tag into a big-endian <c>int</c>, matching the <c>uint</c> conversion.
    /// </summary>
    public static implicit operator int(Tag tag) => (int)(uint)tag;

    /// <summary>
    /// Creates a tag from its 4 raw bytes.
    /// </summary>
    public static implicit operator Tag(Array4<byte> value) => new Tag { _value = value };

    /// <summary>
    /// Gets the tag's 4 raw bytes.
    /// </summary>
    public static implicit operator Array4<byte>(Tag tag) => tag._value;

    internal static void Serialize(BincodeSerializer serializer, Tag value) => value.Serialize(serializer);

    internal void Serialize(BincodeSerializer serializer)
    {
        EguiMarshal.SerializerCache<Array4<byte>>.Serialize(serializer, _value);
    }

    internal static Tag Deserialize(BincodeDeserializer deserializer)
    {
        return new Tag { _value = EguiMarshal.SerializerCache<Array4<byte>>.Deserialize(deserializer) };
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Tag other && Equals(other);

    /// <inheritdoc/>
    public static bool operator ==(Tag left, Tag right) => Equals(left, right);

    /// <inheritdoc/>
    public static bool operator !=(Tag left, Tag right) => !Equals(left, right);

    /// <inheritdoc/>
    public bool Equals(Tag other) => _value.Equals(other._value);

    /// <inheritdoc/>
    public override int GetHashCode() => _value.GetHashCode();

    /// <inheritdoc/>
    public override string ToString() => this;
}
