namespace Egui;

public partial record struct Atom
{
    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator Atom(AtomKind value) => new Atom { Kind = value };

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator Atom(string value) => new Atom { Kind = value };

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator Atom(RichText value) => new Atom { Kind = value };

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator Atom(WidgetText value) => new Atom { Kind = value };

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator Atom(Egui.Text.LayoutJob value) => new Atom { Kind = value };

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator Atom(Egui.Galley value) => new Atom { Kind = value };

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator Atom(ImageSource value) => new Atom { Kind = value };

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator Atom(Egui.Widgets.Image value) => new Atom { Kind = value };
}