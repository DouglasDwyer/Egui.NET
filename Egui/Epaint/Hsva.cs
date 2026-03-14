using System.Collections.Immutable;

namespace Egui.Epaint;

public partial struct Hsva
{
    public static Hsva FromAdditiveSrgb(Array3<byte> rgb)
    {
        return EguiMarshal.Call<Array3<byte>, Hsva>(EguiFn.ecolor_hsva_Hsva_from_additive_srgb, rgb);
    }

    public static Hsva FromSrgb(Array3<byte> rgb)
    {
        return EguiMarshal.Call<Array3<byte>, Hsva>(EguiFn.ecolor_hsva_Hsva_from_srgb, rgb);
    }

    public static Hsva FromSrgbaUnmultiplied(Array4<byte> rgb)
    {
        return EguiMarshal.Call<Array4<byte>, Hsva>(EguiFn.ecolor_hsva_Hsva_from_srgba_unmultiplied, rgb);
    }

    public static Hsva FromSrgbaPremultiplied(Array4<byte> rgb)
    {
        return EguiMarshal.Call<Array4<byte>, Hsva>(EguiFn.ecolor_hsva_Hsva_from_srgba_premultiplied, rgb);
    }
}