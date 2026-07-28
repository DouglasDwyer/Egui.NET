namespace Egui;

public partial record struct TextureOptions
{
    /// <summary>
    /// Linear magnification and minification.
    /// </summary>
    public static readonly TextureOptions Linear = new TextureOptions
    {
        Magnification = TextureFilter.Linear,
        Minification = TextureFilter.Linear,
    };

    /// <summary>
    /// Nearest magnification and minification.
    /// </summary>
    public static readonly TextureOptions Nearest = new TextureOptions
    {
        Magnification = TextureFilter.Nearest,
        Minification = TextureFilter.Nearest,
    };

    /// <summary>
    /// Linear magnification and minification, but with the texture repeated.
    /// </summary>
    public static readonly TextureOptions LinearRepeat = new TextureOptions
    {
        Magnification = TextureFilter.Linear,
        Minification = TextureFilter.Linear,
        WrapMode = TextureWrapMode.Repeat
    };

    /// <summary>
    /// Linear magnification and minification, but with the texture mirrored and repeated.
    /// </summary>
    public static readonly TextureOptions LinearMirroredRepeat = new TextureOptions
    {
        Magnification = TextureFilter.Linear,
        Minification = TextureFilter.Linear,
        WrapMode = TextureWrapMode.MirroredRepeat
    };

    /// <summary>
    /// Nearest magnification and minification, but with the texture repeated.
    /// </summary>
    public static readonly TextureOptions NearestRepeat = new TextureOptions
    {
        Magnification = TextureFilter.Nearest,
        Minification = TextureFilter.Nearest,
        WrapMode = TextureWrapMode.Repeat
    };

    /// <summary>
    /// Nearest magnification and minification, but with the texture mirrored and repeated.
    /// </summary>
    public static readonly TextureOptions NearestMirroredRepeat = new TextureOptions
    {
        Magnification = TextureFilter.Nearest,
        Minification = TextureFilter.Nearest,
        WrapMode = TextureWrapMode.MirroredRepeat
    };
}