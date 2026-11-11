using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Egui;

public partial record struct SizedContainerAtom
{
    /// <summary>
    /// Iterates over the images among the sized atoms.
    /// </summary>
    public readonly IEnumerable<Image> Images => _sizedAtoms.Where(x => x.Kind is SizedAtomKind.Image)
        .Select(x => x.Kind.TryGetValue(out SizedAtomKind.Image image) ? image.Inner : default);

    /// <summary>
    /// Iterates over the texts among the sized atoms.
    /// </summary>
    public readonly IEnumerable<Galley> Texts => _sizedAtoms.Where(x => x.Kind is SizedAtomKind.Text)
        .Select(x => x.Kind.TryGetValue(out SizedAtomKind.Text text) ? text.Value : default!);

    /// <summary>
    /// Iterates over the kinds of the sized atoms.
    /// </summary>
    public readonly IEnumerable<SizedAtomKind> Kinds => _sizedAtoms.Select(x => x.Kind);

    /// <summary>
    /// Replaces the kind of every sized atom with the result of <paramref name="f"/>.
    /// </summary>
    public void MapKind(Func<SizedAtomKind, SizedAtomKind> f)
    {
        _sizedAtoms = _sizedAtoms.Select(x =>
        {
            x.Kind = f(x.Kind);
            return x;
        }).ToImmutableArray();
    }

    /// <summary>
    /// Replaces every image among the sized atoms with the result of <paramref name="f"/>.
    /// </summary>
    public void MapImages(Func<Image, Image> f)
    {
        _sizedAtoms = _sizedAtoms.Select(x =>
        {
            if (x.Kind is SizedAtomKind.Image image)
            {
                x.Kind = new SizedAtomKind.Image
                {
                    Inner = f(image.Inner),
                    Size = image.Size
                };
            }
            return x;
        }).ToImmutableArray();
    }
}
