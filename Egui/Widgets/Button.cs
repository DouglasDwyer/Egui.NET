namespace Egui.Widgets;

public partial record struct Button
{
    /// <summary>
    /// Sets the CSS-like classes for this button.<br/>
    ///
    /// This can be used by a styling engine to compute a different style based on the set of
    /// classes present on the widget.
    /// </summary>
    public void SetClasses(Classes classes)
    {
        _classes = classes;
    }
}
