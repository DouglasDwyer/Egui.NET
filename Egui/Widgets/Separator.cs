namespace Egui.Widgets;

public partial struct Separator
{
    /// <summary>
    /// Sets the CSS-like classes for this separator.<br/>
    ///
    /// This can be used by a styling engine to compute a different style based on the set of
    /// classes present on the widget.
    /// </summary>
    public void SetClasses(Classes classes)
    {
        _classes = classes;
    }
}
