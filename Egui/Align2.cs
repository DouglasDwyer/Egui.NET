namespace Egui;

public partial struct Align2
{
    public static readonly Align2 LeftBottom = new Align2(Align.Min, Align.Max);
    public static readonly Align2 LeftCenter = new Align2(Align.Min, Align.Center);
    public static readonly Align2 LeftTop = new Align2(Align.Min, Align.Min);
    public static readonly Align2 CenterBottom = new Align2(Align.Center, Align.Max);
    public static readonly Align2 CenterCenter = new Align2(Align.Center, Align.Center);
    public static readonly Align2 CenterTop = new Align2(Align.Center, Align.Min);
    public static readonly Align2 RightBottom = new Align2(Align.Max, Align.Max);
    public static readonly Align2 RightCenter = new Align2(Align.Max, Align.Center);
    public static readonly Align2 RightTop = new Align2(Align.Max, Align.Min);

    /// <summary>
    /// Creates a new 2D align with the given values.
    /// </summary>
    public Align2(Align horizontal, Align vertical)
    {
        _value = new Array2<Align>(horizontal, vertical);
    }
}