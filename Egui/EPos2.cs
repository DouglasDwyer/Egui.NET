namespace Egui;

public partial record struct EPos2
{
    /// <summary>
    /// The zero position, the origin. The top left corner in a GUI. Same as <see cref="EPos2()"/>.
    /// </summary>
    public static readonly EPos2 Zero = new EPos2();

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator EPos2((float, float) xy) => new EPos2(xy.Item1, xy.Item2);

    /// <summary>
    /// Performs the <c>+</c> operation.
    /// </summary>
    public static EPos2 operator +(EPos2 lhs, EVec2 rhs) => new EPos2(lhs.X + rhs.X, lhs.Y + rhs.Y);

    /// <summary>
    /// Performs the <c>-</c> operation.
    /// </summary>
    public static EPos2 operator -(EPos2 lhs, EVec2 rhs) => new EPos2(lhs.X - rhs.X, lhs.Y - rhs.Y);

    /// <summary>
    /// Performs the <c>*</c> operation.
    /// </summary>
    public static EPos2 operator *(EPos2 lhs, float rhs) => new EPos2(lhs.X * rhs, lhs.Y * rhs);

    /// <summary>
    /// Performs the <c>*</c> operation.
    /// </summary>
    public static EPos2 operator *(float lhs, EPos2 rhs) => new EPos2(lhs * rhs.X, lhs * rhs.Y);

    /// <summary>
    /// Performs the <c>/</c> operation.
    /// </summary>
    public static EPos2 operator /(EPos2 lhs, float rhs) => new EPos2(lhs.X / rhs, lhs.Y / rhs);
}