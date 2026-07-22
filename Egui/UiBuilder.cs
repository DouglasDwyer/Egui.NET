namespace Egui;

public partial struct UiBuilder
{
    /// <summary>
    /// Sets the CSS-like classes for the <see cref="Ui"/> created from this builder.<br/>
    ///
    /// This can be used by a styling engine to compute a different style based on the set of
    /// classes present on the <see cref="Ui"/>.
    /// </summary>
    public void SetClasses(Classes classes)
    {
        Classes = classes;
    }
}
