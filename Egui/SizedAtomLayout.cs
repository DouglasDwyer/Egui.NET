using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Egui;

public partial record struct SizedAtomLayout
{
    /// <summary>
    /// The auto-generated <c>_sizedAtoms</c> field is private (it mirrors the private Rust
    /// field), so this internal property is the only way for other types in this assembly -
    /// namely <see cref="AllocatedAtomLayout"/>'s <c>Images</c>/<c>Texts</c>/<c>Kinds</c>/
    /// <c>MapKind</c>/<c>MapImages</c> - to read or replace the sized atoms.
    /// </summary>
    internal ImmutableArray<SizedAtom> SizedAtoms
    {
        readonly get => _sizedAtoms;
        set => _sizedAtoms = value;
    }

    /// <summary>
    /// Iterates over the images among the sized atoms.
    /// </summary>
    public readonly IEnumerable<Image> Images => ImagesOf(SizedAtoms);

    /// <summary>
    /// Iterates over the texts among the sized atoms.
    /// </summary>
    public readonly IEnumerable<Galley> Texts => TextsOf(SizedAtoms);

    private static IEnumerable<Image> ImagesOf(ImmutableArray<SizedAtom> atoms)
    {
        foreach (var atom in atoms)
        {
            if (atom.Kind is SizedAtomKind.Image image)
            {
                yield return image.Inner;
            }
        }
    }

    private static IEnumerable<Galley> TextsOf(ImmutableArray<SizedAtom> atoms)
    {
        foreach (var atom in atoms)
        {
            if (atom.Kind is SizedAtomKind.Text text)
            {
                yield return text.Value;
            }
        }
    }

    /// <summary>
    /// Iterates over the kinds of the sized atoms.
    /// </summary>
    public readonly IEnumerable<SizedAtomKind> Kinds => SizedAtoms.Select(x => x.Kind);

    /// <summary>
    /// Replaces the kind of every sized atom with the result of <paramref name="f"/>.
    /// </summary>
    public void MapKind(Func<SizedAtomKind, SizedAtomKind> f)
    {
        SizedAtoms = SizedAtoms.Select(x =>
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
        SizedAtoms = SizedAtoms.Select(x =>
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
