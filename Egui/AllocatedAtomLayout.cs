using System.Collections.Immutable;

namespace Egui;

public partial struct AllocatedAtomLayout
{
    public readonly IEnumerable<Image> Images => Sized.SizedAtoms.Where(x => x.Kind.Inner is SizedAtomKind.Image)
        .Select(x => ((SizedAtomKind.Image)x.Kind.Inner).Inner);

    public readonly IEnumerable<Galley> Texts => Sized.SizedAtoms.Where(x => x.Kind.Inner is SizedAtomKind.Text)
        .Select(x => ((SizedAtomKind.Text)x.Kind.Inner).Value);

    public readonly IEnumerable<SizedAtomKind> Kinds => Sized.SizedAtoms.Select(x => x.Kind);

    public void MapKind(Func<SizedAtomKind, SizedAtomKind> f)
    {
        Sized.SizedAtoms = Sized.SizedAtoms.Select(x =>
        {
            x.Kind = f(x.Kind);
            return x;
        }).ToImmutableArray();
    }

    public void MapImages(Func<Image, Image> f)
    {
        Sized.SizedAtoms = Sized.SizedAtoms.Select(x =>
        {
            if (x.Kind.Inner is SizedAtomKind.Image image)
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
