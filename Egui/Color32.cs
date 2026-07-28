using System.Drawing;

namespace Egui;

/// <summary>
/// This format is used for space-efficient color representation (32 bits).
///
/// Instead of manipulating this directly it is often better
/// to first convert it to either <c>Rgba</c> or <c>Hsva</c>.
///
/// Internally this uses 0-255 gamma space <c>sRGBA</c> color with _premultiplied alpha_.
///
/// It's the non-linear ("gamma") values that are multiplied with the alpha.
///
/// Premultiplied alpha means that the color values have been pre-multiplied with the alpha (opacity).
/// This is in contrast with "normal" RGBA, where the alpha is _separate_ (or "unmultiplied").
/// Using premultiplied alpha has some advantages:
/// * It allows encoding additive colors
/// * It is the better way to blend colors, e.g. when filtering texture colors
/// * Because the above, it is the better way to encode colors in a GPU texture
///
/// The color space is assumed to be <c>sRGB</c>.
///
/// All operations on <see cref="Color32"/>  are done in "gamma space".
/// This is not physically correct, but it is fast and sometimes more perceptually even than linear space.
/// If you instead want to perform these operations in linear-space color, use <c>Rgba</c>.
///
/// An <c>alpha=0</c> means the color is to be treated as an additive color.
/// </summary>
public unsafe partial record struct Color32
{
    public static readonly Color32 Transparent = FromRgbaPremultiplied(0, 0, 0, 0);
    public static readonly Color32 Black = FromRgb(0, 0, 0);
    public static readonly Color32 DarkGray = FromRgb(96, 96, 96);
    public static readonly Color32 Gray = FromRgb(160, 160, 160);
    public static readonly Color32 LightGray = FromRgb(220, 220, 220);
    public static readonly Color32 White = FromRgb(255, 255, 255);

    public static readonly Color32 Brown = FromRgb(165, 42, 42);
    public static readonly Color32 DarkRed = FromRgb(0x8B, 0, 0);
    public static readonly Color32 Red = FromRgb(255, 0, 0);
    public static readonly Color32 LightRed = FromRgb(255, 128, 128);

    public static readonly Color32 Cyan = FromRgb(0, 255, 255);
    public static readonly Color32 Magenta = FromRgb(255, 0, 255);
    public static readonly Color32 Yellow = FromRgb(255, 255, 0);

    public static readonly Color32 Orange = FromRgb(255, 165, 0);
    public static readonly Color32 LightYellow = FromRgb(255, 255, 0xE0);
    public static readonly Color32 Khaki = FromRgb(240, 230, 140);

    public static readonly Color32 DarkGreen = FromRgb(0, 0x64, 0);
    public static readonly Color32 Green = FromRgb(0, 255, 0);
    public static readonly Color32 LightGreen = FromRgb(0x90, 0xEE, 0x90);

    public static readonly Color32 DarkBlue = FromRgb(0, 0, 0x8B);
    public static readonly Color32 Blue = FromRgb(0, 0, 255);
    public static readonly Color32 LightBlue = FromRgb(0xAD, 0xD8, 0xE6);

    public static readonly Color32 Purple = FromRgb(0x80, 0, 0x80);

    public static readonly Color32 Gold = FromRgb(255, 215, 0);

    public static readonly Color32 DebugColor = FromRgbaPremultiplied(0, 200, 0, 128);

    /// <summary>
    /// An ugly color that is planned to be replaced before making it to the screen.
    ///
    /// This is an invalid color, in that it does not correspond to a valid multiplied color,
    /// nor to an additive color.
    ///
    /// This is used as a special color key,
    /// i.e. often taken to mean "no color".
    /// </summary>
    public static readonly Color32 Placeholder = FromRgbaPremultiplied(64, 254, 0, 128);

    /// <summary>
    /// The underlying encoded color.
    /// </summary>
    private uint _value;

    /// <summary>
    /// Parses a color from a hex string.<br/>
    /// Supports the 3, 4, 6, and 8-digit formats, according to the specification in https://drafts.csswg.org/css-color-4/#hex-color
    /// </summary>
    /// <exception cref="ArgumentException">Returns an error if the length of the string does not correspond to one of the standard formats (3, 4, 6, or 8), or if it contains non-hex characters.</exception>
    public static Color32 FromHex(string hex)
    {
        var result = EguiMarshal.Call<string, Color32?>(EguiFn.ecolor_color32_Color32_from_hex, hex);
        if (result.HasValue)
        {
            return result.Value;
        }
        else
        {
            throw new ArgumentException($"Failed to parse '{hex}' as hex string", nameof(hex));
        }
    }

    /// <summary>
    /// Serializes an instance of this type.
    /// </summary>
    internal static void Serialize(BincodeSerializer serializer, Color32 value) => value.Serialize(serializer);

    /// <summary>
    /// Serializes this object.
    /// </summary>
    internal void Serialize(BincodeSerializer serializer)
    {
        serializer.increase_container_depth();
        serializer.serialize_u32(_value);
        serializer.decrease_container_depth();
    }

    /// <summary>
    /// Deserializes an instance of this type.
    /// </summary>
    internal static Color32 Deserialize(BincodeDeserializer deserializer)
    {
        deserializer.increase_container_depth();
        Color32 obj = default;
        obj._value = deserializer.deserialize_u32();
        deserializer.decrease_container_depth();
        return obj;
    }

    /// <summary>
    /// Restricts the compiler-synthesized <see cref="ToString"/> to just the packed
    /// color value, rather than every convenience property defined on this type (some of
    /// which call into native egui code).
    /// </summary>
    private readonly bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append("Value = ").Append(_value);
        return true;
    }
}