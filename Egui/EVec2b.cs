namespace Egui;

public partial record struct EVec2b
{
    public static readonly EVec2b False = new EVec2b(false, false);

    public static readonly EVec2b True = new EVec2b(true, true);

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator EVec2b(bool v) => new EVec2b(v, v);

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator EVec2b((bool, bool) v) => new EVec2b(v.Item1, v.Item2);
}