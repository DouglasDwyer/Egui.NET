namespace Egui.Containers;

public struct ScrollAreaOutput
{
    /// <inheritdoc cref="ScrollAreaOutput{R}.Id"/>
    public required Id Id;

    /// <inheritdoc cref="ScrollAreaOutput{R}.State"/>
    public required State State;

    /// <inheritdoc cref="ScrollAreaOutput{R}.ContentSize"/>
    public required EVec2 ContentSize;

    /// <inheritdoc cref="ScrollAreaOutput{R}.InnerRect"/>
    public required Rect InnerRect;
}

public struct ScrollAreaOutput<R>
{
    /// <summary>
    /// What the user closure returned.
    /// </summary>
    public required R Inner;

    /// <summary>
    /// <see cref="Id"/> of the <see cref="ScrollArea"/>.  
    /// </summary>
    public required Id Id;

    /// <summary>
    /// The current state of the scroll area.
    /// </summary>
    public required State State;

    /// <summary>
    /// The size of the content. If this is larger than <see cref="InnerRect"/>, then there was
    /// need for scrolling.
    /// </summary>
    public required EVec2 ContentSize;

    /// <summary>
    /// Where on the screen the content is (excludes scroll bars).
    /// </summary>
    public required Rect InnerRect;

    /// <summary>
    /// Strips the generic argument.
    /// </summary>
    internal readonly ScrollAreaOutput WithoutInner()
    {
        return new ScrollAreaOutput
        {
            InnerRect = InnerRect,
            Id = Id,
            State = State,
            ContentSize = ContentSize,
        };
    }
}