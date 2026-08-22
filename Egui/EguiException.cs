namespace Egui;

/// <summary>
/// An exception produced from a native <c>egui</c> call.
/// </summary>
public class EguiException : Exception
{
    /// <summary>
    /// Creates a new exception.
    /// </summary>
    public EguiException() { }

    /// <summary>
    /// Creates a new exception with the specified error message.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public EguiException(string message) : base(message) { }
}