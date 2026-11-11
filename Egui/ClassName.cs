namespace Egui;

public partial record struct ClassName
{
    /// <summary>
    /// A class from a string.
    /// </summary>
    public ClassName(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Converts a string into a class.
    /// </summary>
    public static implicit operator ClassName(string value) => new ClassName(value);

    /// <summary>
    /// The class as a string.
    /// </summary>
    public override readonly string ToString() => _value ?? "";
}
