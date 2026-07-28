namespace Egui;

public partial record struct AllocatedAtomLayout
{
    /// <inheritdoc cref="SizedAtomLayout.Images"/>
    public readonly IEnumerable<Image> Images => Sized.Images;

    /// <inheritdoc cref="SizedAtomLayout.Texts"/>
    public readonly IEnumerable<Galley> Texts => Sized.Texts;

    /// <inheritdoc cref="SizedAtomLayout.Kinds"/>
    public readonly IEnumerable<SizedAtomKind> Kinds => Sized.Kinds;

    /// <inheritdoc cref="SizedAtomLayout.MapKind"/>
    public void MapKind(Func<SizedAtomKind, SizedAtomKind> f) => Sized.MapKind(f);

    /// <inheritdoc cref="SizedAtomLayout.MapImages"/>
    public void MapImages(Func<Image, Image> f) => Sized.MapImages(f);
}
