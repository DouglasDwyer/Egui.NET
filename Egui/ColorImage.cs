using System.Collections.Immutable;

namespace Egui;

public partial record struct ColorImage
{
    /// <summary>
    /// Alternative method to <see cref="FromGray"/>.<br/>
    /// Create a <see cref="ColorImage"/> from iterator over flat opaque gray data. 
    /// </summary>
    /// <exception cref="ArgumentException">If <c>size[0] * size[1] != grayIter.Count</c>.</exception>
    public static ColorImage FromGrayIter(Array2<nuint> size, IEnumerable<byte> grayIter)
    {
        var pixels = grayIter.Select(Color32.FromGray).ToImmutableArray();
        if ((nuint)pixels.Length == size[0] * size[1])
        {
            return new ColorImage(size, pixels);
        }
        else
        {
            throw new ArgumentException($"Image size {size[0]}x{size[1]} did not match enumerator length {pixels.Length}", nameof(size));
        }
    }

    /// <summary>
    /// Clone a sub-region as a new image.
    /// </summary>
    public readonly ColorImage RegionByPixels(ImmutableArray<nuint> xy, ImmutableArray<nuint> wh)
    {
        return EguiMarshal.Call<ColorImage, ImmutableArray<nuint>, ImmutableArray<nuint>, ColorImage>(EguiFn.epaint_image_ColorImage_region_by_pixels, this, xy, wh);
    }
}