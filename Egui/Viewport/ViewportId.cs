namespace Egui.Viewport;

public partial record struct ViewportId
{
    /// <summary>
    /// The <see cref="ViewportId"/> of the root viewport.
    /// </summary>
    public static readonly ViewportId Root = new ViewportId { _value = Id.Null };

    /// <summary>
    /// Creates a <see cref="ViewportId"/> by hashing the given source.
    /// </summary>
    /// <param name="source">The source to hash.</param>
    public static ViewportId FromHashOf(string source)
    {
        return EguiMarshal.Call<string, ViewportId>(EguiFn.egui_viewport_ViewportId_from_hash_of, source);
    }
}