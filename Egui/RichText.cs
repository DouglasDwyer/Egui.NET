namespace Egui;

public partial struct RichText
{
    /// <summary>
    /// Creates a <see cref="RichText"/> object for the provided string.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    public static implicit operator RichText(string value) => new RichText(value);
}