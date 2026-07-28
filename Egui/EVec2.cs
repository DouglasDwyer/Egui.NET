namespace Egui;

public partial record struct EVec2
{
    /// <summary>
    /// +X
    /// </summary>
    public static readonly EVec2 Right = new EVec2(1, 0);

    /// <summary>
    /// -X
    /// </summary>
    public static readonly EVec2 Left = new EVec2(-1, 0);

    /// <summary>
    /// -Y
    /// </summary>
    public static readonly EVec2 Up = new EVec2(0, -1);

    /// <summary>
    /// +Y
    /// </summary>
    public static readonly EVec2 Down = new EVec2(0, 1);

    public static readonly EVec2 Zero = new EVec2(0);

    public static readonly EVec2 One = new EVec2(1);

    public static readonly EVec2 Infinity = new EVec2(float.PositiveInfinity);

    public static readonly EVec2 NaN = new EVec2(float.NaN);

    /// <summary>
    /// Same as <see cref="Splat"/>.
    /// </summary>
    public EVec2(float v) : this(v, v) { }

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator EVec2((float, float) xy) => new EVec2(xy.Item1, xy.Item2);

    /// <summary>
    /// Performs the <c>+</c> operation.
    /// </summary>
    public static EVec2 operator +(EVec2 lhs, EVec2 rhs) => new EVec2(lhs.X + rhs.X, lhs.Y + rhs.Y);

    /// <summary>
    /// Performs the <c>-</c> operation.
    /// </summary>
    public static EVec2 operator -(EVec2 lhs, EVec2 rhs) => new EVec2(lhs.X - rhs.X, lhs.Y - rhs.Y);

    /// <summary>
    /// Performs the <c>*</c> operation.
    /// </summary>
    public static EVec2 operator *(EVec2 lhs, EVec2 rhs) => new EVec2(lhs.X * rhs.X, lhs.Y * rhs.Y);

    /// <summary>
    /// Performs the <c>*</c> operation.
    /// </summary>
    public static EVec2 operator *(EVec2 lhs, float rhs) => new EVec2(lhs.X * rhs, lhs.Y * rhs);

    /// <summary>
    /// Performs the <c>*</c> operation.
    /// </summary>
    public static EVec2 operator *(float lhs, EVec2 rhs) => new EVec2(lhs * rhs.X, lhs * rhs.Y);

    /// <summary>
    /// Performs the <c>*</c> operation.
    /// </summary>
    public static EVec2 operator /(EVec2 lhs, EVec2 rhs) => new EVec2(lhs.X / rhs.X, lhs.Y / rhs.Y);

    /// <summary>
    /// Performs the <c>/</c> operation.
    /// </summary>
    public static EVec2 operator /(EVec2 lhs, float rhs) => new EVec2(lhs.X / rhs, lhs.Y / rhs);
}