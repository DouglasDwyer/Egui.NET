namespace Egui;

public partial record struct ImageSource
{
    /// <summary>
    /// Get the <c>uri</c> that this image was constructed from.<br/>
    /// This will return <c>null</c> for <see cref="Texture"/>. 
    /// </summary>
    public string? GetUri() => this switch
    {
        Bytes bytes => bytes.Uri,
        Uri uri => uri.Value,
        Texture => null,
    };
}