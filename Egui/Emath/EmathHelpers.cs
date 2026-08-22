using System.Numerics;

namespace Egui.Emath;

public static partial class EmathHelpers
{
    /// <summary>
    /// If you have a value animating over time,
    /// how much towards its target do you need to move it this frame?
    ///
    /// You only need to store the start time and target value in order to animate using this function.
    /// </summary>
    public static float InterpolationFactor((double, double) times, double currentTime, float dt, Func<float, float> easing)
    {
        var animationDuration = (float)(times.Item2 - times.Item1);
        var prevTime = currentTime - (double)dt;
        var prevT = easing((float)(prevTime - times.Item1) / animationDuration);
        var endT = easing((float)(currentTime - times.Item1) / animationDuration);
        if (endT < 1.0f)
        {
            return (endT - prevT) / (1.0f - prevT);
        }
        else
        {
            return 1.0f;
        }
    }

    /// <summary>
    /// Linear interpolation.
    /// </summary>
    public static R Lerp<R>((R, R) range, R t) where R : INumber<R>
    {
        return (R.One - t) * range.Item1 + t * range.Item2;
    }

    /// <summary>
    /// The midpoint of <paramref name="a"/> and <paramref name="b"/>.
    /// </summary>
    public static R FastMidpoint<R>(R a, R b) where R : INumber<R>
    {
        return (a + b) / (R.One + R.One);
    }

    /// <summary>
    /// Where in the range is this value? Returns 0-1 if within the range.<br/>
    /// Returns &lt;0 if before and &gt;1 if after.<br/>
    /// Returns <c>null</c> if the input range is zero-width.
    /// </summary>
    public static R? InverseLerp<R>((R, R) range, R value) where R : struct, INumber<R>
    {
        var start = range.Item1;
        var end = range.Item2;
        if (start == end)
        {
            return null;
        }
        else
        {
            return (value - start) / (end - start);
        }
    }

    /// <summary>
    /// Linearly remap a value from one range to another,
    /// so that when <c>x == from.Item1</c> returns <c>to.Item1</c>
    /// and when <c>x == from.Item2</c> returns <c>to.Item2</c>.
    /// </summary>
    public static R Remap<R>(R x, (R, R) from, (R, R) to) where R : INumber<R>
    {
        var t = (x - from.Item1) / (from.Item2 - from.Item1);
        return Lerp(to, t);
    }

    /// <summary>
    /// Like <see cref="Remap"/>, but also clamps the value so that the returned value is always in the <c>to</c> range.
    /// </summary>
    public static R RemapClamp<R>(R x, (R, R) from, (R, R) to) where R : INumber<R>
    {
        if (from.Item2 < from.Item1)
        {
            return RemapClamp(x, (from.Item2, from.Item1), (to.Item2, to.Item1));
        }

        if (x <= from.Item1)
        {
            return to.Item1;
        }
        else if (from.Item2 <= x)
        {
            return to.Item2;
        }
        else
        {
            var t = (x - from.Item1) / (from.Item2 - from.Item1);
            if (R.One <= t)
            {
                return to.Item2;
            }
            else
            {
                return Lerp(to, t);
            }
        }
    }
}