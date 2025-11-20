using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Egui;

/// <summary>
/// Helper to paint shapes and text to a specific region on a specific layer.
// All coordinates are screen coordinates in the unit points (one point can consist of many physical pixels).
/// A <see cref="Painter"/>  never outlive a single frame/pass.
/// </summary>
public sealed partial class Painter : EguiObject
{
    /// <summary>
    /// Get a reference to the parent <see cref="Context"/>. 
    /// </summary>
    public readonly Context Ctx;

    /// <summary>
    /// Create a painter to a specific layer within a certain clip rectangle.
    /// </summary>
    public Painter(Context ctx, LayerId layerId, Rect clipRect) : this(ctx, EguiMarshal.Call<nuint, LayerId, Rect, EguiHandle>(EguiFn.egui_painter_Painter_new, ctx.Ptr, layerId, clipRect)) { }

    /// <summary>
    /// Creates a new painter object from the given handle.
    /// </summary>
    /// <param name="handle">The handle to use.</param>
    internal Painter(Context ctx, EguiHandle handle) : base(handle)
    {
        Ctx = ctx;
    }

    /// <summary>
    /// It is up to the caller to make sure there is room for this. Can be used for free painting. NOTE: all coordinates are screen coordinates!
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public ShapeIdx Add(Shape shape)
    {
        return EguiMarshal.Call<nuint, Shape, ShapeIdx>(EguiFn.egui_painter_Painter_add, Ptr, shape);
    }

    /// <summary>
    /// Add many shapes at once.
    /// Calling this once is generally faster than calling <see cref="Add"/> multiple times
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Extend(IEnumerable<Shape> shapes)
    {
        if (!IsVisible)
        {
            return;
        }

        foreach (var shape in shapes)
        {
            Add(shape);
        }
    }

    /// <summary>
    /// Access all shapes added this frame.
    /// </summary>
    public void ForEachShape(Action<ClippedShape> reader)
    {
        var shapes = EguiMarshal.Call<nuint, ImmutableArray<ClippedShape>>(EguiFn.egui_painter_Painter_for_each_shape, Ptr);
        foreach (var shape in shapes)
        {
            reader(shape);
        }
    }

    /// <summary>
    /// Redirect where you are painting.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Painter WithLayerId(LayerId layerId)
    {
        return new Painter(Ctx, EguiMarshal.Call<nuint, LayerId, EguiHandle>(EguiFn.egui_painter_Painter_with_layer_id, Ptr, layerId));
    }

    /// <summary>
    /// Create a painter for a sub-region of this <see cref="Painter"/>.<br/>
    /// The clip-rect of the returned <see cref="Painter"/> will be the intersection of the given rectangle and the <see cref="ClipRect"/> of the parent <see cref="Painter"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Painter WithClipRect(Rect clipRect)
    {
        return new Painter(Ctx, EguiMarshal.Call<nuint, Rect, EguiHandle>(EguiFn.egui_painter_Painter_with_clip_rect, Ptr, clipRect));
    }

    /// <inheritdoc cref="Context.Fonts"/>
    public void Fonts(Action<FontsView> reader) => Ctx.Fonts(reader);

    /// <inheritdoc cref="Context.Fonts"/>
    public R Fonts<R>(Func<FontsView, R> reader) => Ctx.Fonts(reader);
}