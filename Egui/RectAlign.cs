using System.Collections.Immutable;

namespace Egui;

public partial record struct RectAlign
{
    /// <summary>
    /// Along the top edge, leftmost.
    /// </summary>
    public static readonly RectAlign TopStart = new RectAlign
    {
        Parent = Align2.LeftTop,
        Child = Align2.LeftBottom,
    };

    /// <summary>
    /// Along the top edge, centered.
    /// </summary>
    public static readonly RectAlign Top = new RectAlign
    {
        Parent = Align2.CenterTop,
        Child = Align2.CenterBottom,
    };

    /// <summary>
    /// Along the top edge, rightmost.
    /// </summary>
    public static readonly RectAlign TopEnd = new RectAlign
    {
        Parent = Align2.RightTop,
        Child = Align2.RightBottom,
    };

    /// <summary>
    /// Along the right edge, topmost.
    /// </summary>
    public static readonly RectAlign RightStart = new RectAlign
    {
        Parent = Align2.RightTop,
        Child = Align2.LeftTop,
    };

    /// <summary>
    /// Along the right edge, centered.
    /// </summary>
    public static readonly RectAlign Right = new RectAlign
    {
        Parent = Align2.RightCenter,
        Child = Align2.LeftCenter,
    };

    /// <summary>
    /// Along the right edge, bottommost.
    /// </summary>
    public static readonly RectAlign RightEnd = new RectAlign
    {
        Parent = Align2.RightBottom,
        Child = Align2.LeftBottom,
    };

    /// <summary>
    /// Along the bottom edge, rightmost.
    /// </summary>
    public static readonly RectAlign BottomEnd = new RectAlign
    {
        Parent = Align2.RightBottom,
        Child = Align2.RightTop,
    };

    /// <summary>
    /// Along the bottom edge, centered.
    /// </summary>
    public static readonly RectAlign Bottom = new RectAlign
    {
        Parent = Align2.CenterBottom,
        Child = Align2.CenterTop,
    };

    /// <summary>
    /// Along the bottom edge, leftmost.
    /// </summary>
    public static readonly RectAlign BottomStart = new RectAlign
    {
        Parent = Align2.LeftBottom,
        Child = Align2.LeftTop,
    };

    /// <summary>
    /// Along the left edge, bottommost.
    /// </summary>
    public static readonly RectAlign LeftEnd = new RectAlign
    {
        Parent = Align2.LeftBottom,
        Child = Align2.RightBottom,
    };

    /// <summary>
    /// Along the left edge, centered.
    /// </summary>
    public static readonly RectAlign Left = new RectAlign
    {
        Parent = Align2.LeftCenter,
        Child = Align2.RightCenter,
    };

    /// <summary>
    /// Along the left edge, topmost.
    /// </summary>
    public static readonly RectAlign LeftStart = new RectAlign
    {
        Parent = Align2.LeftTop,
        Child = Align2.RightTop,
    };

    /// <summary>
    /// The 12 most common menu positions as an array, for use with <see cref="FindBestAlign"/>.
    /// </summary>
    public static ImmutableArray<RectAlign> MenuAligns => [
        BottomStart,
        BottomEnd,
        TopStart,
        TopEnd,
        RightEnd,
        RightStart,
        LeftEnd,
        LeftStart,
        // These come last on purpose, we prefer the corner ones
        Top,
        Right,
        Bottom,
        Left
    ];

    /// <summary>
    /// Look for the first alternative <see cref="RectAlign"/>  that allows the child rect to fit
    /// inside the <c>screenRect</c>.
    ///
    /// If no alternative fits, the first is returned.
    /// If no alternatives are given, <c>null</c> is returned.
    /// </summary>
    public static RectAlign? FindBestAlign(IEnumerable<RectAlign> valuesToTry, Rect screenRect, Rect parentRect, float gap, EVec2 expectedSize)
    {
        return EguiMarshal.Call<ImmutableArray<RectAlign>, Rect, Rect, float, EVec2, RectAlign?>(EguiFn.emath_rect_align_RectAlign_find_best_align,
            valuesToTry.ToImmutableArray(), screenRect, parentRect, gap, expectedSize);
    }
}