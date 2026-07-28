namespace Egui.Ecolor;

public partial record struct HexColor
{
    /// <summary>
    /// Parses a string <paramref name="s"/> to return a value of this type.
    /// </summary>
    /// <exception cref="ArgumentException">Returns an error if the length of the string does not correspond to one of the standard formats (3, 4, 6, or 8), or if it contains non-hex characters.</exception>
    public HexColor FromStr(string s)
    {
        var result = EguiMarshal.Call<string, HexColor?>(EguiFn.ecolor_hex_color_runtime_HexColor_from_str, s);
        if (result.HasValue)
        {
            return result.Value;
        }
        else
        {
            throw new ArgumentException($"Failed to parse '{s}' as hex string", nameof(s));
        }
    }

    /// <summary>
    /// Parses a string as a hex color without the leading # character
    /// </summary>
    /// <exception cref="ArgumentException">Returns an error if the length of the string does not correspond to one of the standard formats (3, 4, 6, or 8), or if it contains non-hex characters.</exception>
    public HexColor FromStrWithoutHash(string s)
    {
        var result = EguiMarshal.Call<string, HexColor?>(EguiFn.ecolor_hex_color_runtime_HexColor_from_str_without_hash, s);
        if (result.HasValue)
        {
            return result.Value;
        }
        else
        {
            throw new ArgumentException($"Failed to parse '{s}' as hex string", nameof(s));
        }
    }
}