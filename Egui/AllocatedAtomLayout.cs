namespace Egui;

public partial record struct AllocatedAtomLayout
{
    /// <inheritdoc cref="SizedContainerAtom.Images"/>
    public readonly IEnumerable<Image> Images => Container.Images;

    /// <inheritdoc cref="SizedContainerAtom.Texts"/>
    public readonly IEnumerable<Galley> Texts => Container.Texts;

    /// <inheritdoc cref="SizedContainerAtom.Kinds"/>
    public readonly IEnumerable<SizedAtomKind> Kinds => Container.Kinds;

    /// <inheritdoc cref="SizedContainerAtom.MapKind"/>
    public void MapKind(Func<SizedAtomKind, SizedAtomKind> f) => Container.MapKind(f);

    /// <inheritdoc cref="SizedContainerAtom.MapImages"/>
    public void MapImages(Func<Image, Image> f) => Container.MapImages(f);
}
