namespace Egui.Epaint;

/// <summary>
/// The context’s collection of fonts, with this context's <see cref="Context.PixelsPerPoint"/>. This is what you use to do text layout.
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

    /// <summary>
    /// Will wrap text at the given width and line break at <c>\n</c>.<br/>
    ///
    /// The implementation uses memoization so repeated calls are cheap.
    /// </summary>
    public Galley Layout(string text, Egui.FontId fontId, Egui.Color32 color, float wrapWidth)
        => new Galley(EguiMarshal.Call<nuint, string, Egui.FontId, Egui.Color32, float, EguiHandle>(EguiFn.epaint_text_fonts_FontsView_layout, Ptr, text, fontId, color, wrapWidth));

    /// <summary>
    /// Will line break at <c>\n</c>.<br/>
    ///
    /// The implementation uses memoization so repeated calls are cheap.
    /// </summary>
    public Galley LayoutNoWrap(string text, Egui.FontId fontId, Egui.Color32 color)
        => new Galley(EguiMarshal.Call<nuint, string, Egui.FontId, Egui.Color32, EguiHandle>(EguiFn.epaint_text_fonts_FontsView_layout_no_wrap, Ptr, text, fontId, color));

    /// <summary>
    /// Lays out text using a full <see cref="Egui.Text.LayoutJob"/>. The implementation uses
    /// memoization so repeated calls are cheap.
    /// </summary>
    public Galley LayoutJob(Egui.Text.LayoutJob job)
        => new Galley(EguiMarshal.Call<nuint, Egui.Text.LayoutJob, EguiHandle>(EguiFn.epaint_text_fonts_FontsView_layout_job, Ptr, job));

    /// <summary>
    /// Like <see cref="Layout"/>, made for when you want to pick a color for the text later.<br/>
    ///
    /// The implementation uses memoization so repeated calls are cheap.
    /// </summary>
    public Galley LayoutDelayedColor(string text, Egui.FontId fontId, float wrapWidth)
        => new Galley(EguiMarshal.Call<nuint, string, Egui.FontId, float, EguiHandle>(EguiFn.epaint_text_fonts_FontsView_layout_delayed_color, Ptr, text, fontId, wrapWidth));
}