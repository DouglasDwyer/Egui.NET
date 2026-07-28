namespace Egui;

public partial record struct Rect
{
    /// <summary>
    /// Infinite rectangle that contains every point.
    /// </summary>
    public static readonly Rect Everything = Rect.FromMinMax(new EPos2(float.NegativeInfinity, float.NegativeInfinity), new EPos2(float.PositiveInfinity, float.PositiveInfinity));

    /// <summary>
    /// The inverse of <see cref="Everything"/> : stretches from positive infinity to negative infinity.
    /// Contains no points.><br/>
    ///
    /// This is useful as the seed for bounding boxes.
    /// </summary>
    public static readonly Rect Nothing = Rect.FromMinMax(new EPos2(float.PositiveInfinity, float.PositiveInfinity), new EPos2(float.NegativeInfinity, float.NegativeInfinity));

    /// <summary>
    /// An invalid <see cref="Rect"/> filled with <see cref="float.NaN"/>. 
    /// </summary>
    public static readonly Rect NaN = Rect.FromMinMax(new EPos2(float.NaN, float.NaN), new EPos2(float.NaN, float.NaN));

    /// <summary>
    /// A <see cref="Rect"/> filled with zeroes. 
    /// </summary>
    public static readonly Rect Zero = Rect.FromMinMax(new EPos2(0, 0), new EPos2(0, 0));

    /// <summary>
    /// <c>Max.y</c>
    /// </summary>
    public float Bottom
    {
        readonly get => Max.Y;
        set => Max.Y = value;
    }

    public EPos2 Center
    {
        readonly get => EguiMarshal.Call<Rect, EPos2>(EguiFn.emath_rect_Rect_center, this);
        set => this = EguiMarshal.Call<Rect, EPos2, Rect>(EguiFn.emath_rect_Rect_center, this, value);
    }

    /// <summary>
    /// Note: this can be negative.
    /// </summary>
    public float Height
    {
        readonly get => Max.Y - Min.Y;
        set => Max.Y = Min.Y + value;
    }

    /// <summary>
    /// Note: this can be negative.
    /// </summary>
    public float Width
    {
        readonly get => Max.X - Min.X;
        set => Max.Y = Min.X + value;
    }

    /// <summary>
    /// <c>Min.x</c>
    /// </summary>
    public float Left
    {
        readonly get => Min.X;
        set => Min.X = value;
    }

    /// <summary>
    /// <c>Max.x</c>
    /// </summary>
    public float Right
    {
        readonly get => Max.X;
        set => Max.X = value;
    }

    /// <summary>
    /// <c>Min.y</c>
    /// </summary>
    public float Top
    {
        readonly get => Min.Y;
        set => Min.Y = value;
    }
}