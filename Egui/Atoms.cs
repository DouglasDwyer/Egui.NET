using System.Collections;
using System.Collections.Immutable;

namespace Egui;

public partial record struct Atoms : IEnumerable<Atom>
{
    public IEnumerable<AtomKind> Kinds => this.Select(x => x.Kind);

    public IEnumerable<Image> Images => _value.Where(x => x.Kind.Inner is AtomKind.Image)
        .Select(x => ((AtomKind.Image)x.Kind.Inner).Value);

    public IEnumerable<WidgetText> Texts => _value.Where(x => x.Kind.Inner is AtomKind.Text)
        .Select(x => ((AtomKind.Text)x.Kind.Inner).Value);

    /// <summary>
    /// Concatenate and return the text contents.
    /// </summary>
    public string? Text
    {
        get
        {
            var (hasText, text) = EguiMarshal.Call<Atoms, (bool, string)>(EguiFn.egui_atomics_atoms_Atoms_text, this);
            return hasText ? text : null;
        }
    }

    /// <summary>
    /// Creates a value from an iterator.
    /// </summary>
    public Atoms(IEnumerable<Atom> atoms)
    {
        var result = new Atoms();
        foreach (var atom in atoms)
        {
            result.PushRight(atom);
        }
        this = result;
    }

    public void MapAtoms(Func<Atom, Atom> f)
    {
        this = new Atoms(this.Select(f));
    }

    public void MapKind(Func<AtomKind, AtomKind> f)
    {
        this = new Atoms(this.Select(x =>
        {
            x.Kind = f(x.Kind);
            return x;
        }));
    }

    public void MapImages(Func<Image, Image> f)
    {
        _value = _value.Select(x =>
        {
            if (x.Kind.Inner is AtomKind.Image image)
            {
                x.Kind = new AtomKind.Image(f(image.Value));
            }
            return x;
        }).ToImmutableArray();
    }

    public void MapTexts(Func<WidgetText, WidgetText> f)
    {
        _value = _value.Select(x =>
        {
            if (x.Kind.Inner is AtomKind.Text text)
            {
                x.Kind = new AtomKind.Text(f(text.Value));
            }
            return x;
        }).ToImmutableArray();
    }

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator Atoms(Atom value) => FromSingle(value);

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator Atoms(string value) => FromSingle(value);

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator Atoms(RichText value) => FromSingle(value);

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator Atoms(WidgetText value) => FromSingle(value);

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator Atoms(LayoutJob value) => FromSingle(value);

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator Atoms(Galley value) => FromSingle(value);

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator Atoms(ImageSource value) => FromSingle(value);

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator Atoms(Image value) => FromSingle(value);

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator Atoms((Atom, Atom) value)
    {
        var result = new Atoms();
        result.PushRight(value.Item1);
        result.PushRight(value.Item2);
        return result;
    }

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator Atoms((Atom, Atom, Atom) value)
    {
        var result = new Atoms();
        result.PushRight(value.Item1);
        result.PushRight(value.Item2);
        result.PushRight(value.Item3);
        return result;
    }

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator Atoms((Atom, Atom, Atom, Atom) value)
    {
        var result = new Atoms();
        result.PushRight(value.Item1);
        result.PushRight(value.Item2);
        result.PushRight(value.Item3);
        result.PushRight(value.Item4);
        return result;
    }

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator Atoms((Atom, Atom, Atom, Atom, Atom) value)
    {
        var result = new Atoms();
        result.PushRight(value.Item1);
        result.PushRight(value.Item2);
        result.PushRight(value.Item3);
        result.PushRight(value.Item4);
        result.PushRight(value.Item5);
        return result;
    }

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator Atoms((Atom, Atom, Atom, Atom, Atom, Atom) value)
    {
        var result = new Atoms();
        result.PushRight(value.Item1);
        result.PushRight(value.Item2);
        result.PushRight(value.Item3);
        result.PushRight(value.Item4);
        result.PushRight(value.Item5);
        result.PushRight(value.Item6);
        return result;
    }

    /// <summary>
    /// Creates a set of atoms for the given single atom.
    /// </summary>
    private static Atoms FromSingle(Atom atom)
    {
        var result = new Atoms();
        result.PushRight(atom);
        return result;
    }

    /// <inheritdoc/>
    public IEnumerator<Atom> GetEnumerator()
    {
        return ((IEnumerable<Atom>)_value).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}