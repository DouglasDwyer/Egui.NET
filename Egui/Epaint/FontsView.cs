namespace Egui.Epaint;

/// <summary>
/// The context’s collection of fonts, with this context's <see cref="Context.PixelsPerPoint">. This is what you use to do text layout.
/// </summary>
public ref partial struct FontsView
{
    /// <summary>
    /// A pointer to the underlying fonts object.
    /// </summary>
    internal readonly nuint Ptr;

    /// <summary>
    /// Initializes this object.
    /// </summary>
    /// <param name="ptr">The pointer representing the object.</param>
    internal FontsView(nuint ptr)
    {
        Ptr = ptr;
    }
}