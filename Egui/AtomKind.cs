namespace Egui;

public partial record struct AtomKind
{
    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator AtomKind(string value) => new AtomKind.Text(value);

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator AtomKind(RichText value) => new AtomKind.Text(value);

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator AtomKind(WidgetText value) => new AtomKind.Text(value);
    
    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator AtomKind(Egui.Text.LayoutJob value) => new AtomKind.Text(value);
    
    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator AtomKind(Egui.Galley value) => new AtomKind.Text(value);

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator AtomKind(ImageSource value) => new AtomKind.Image(value);

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator AtomKind(Egui.Widgets.Image value) => new AtomKind.Image(value);
}