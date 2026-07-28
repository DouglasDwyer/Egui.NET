namespace Egui.Widgets;

public partial record struct ImageSource
{
    /// <summary>
    /// Get the <c>uri</c> that this image was constructed from.<br/>
    /// This will return <c>null</c> for <see cref="Texture"/>. 
    /// </summary>
    public string? GetUri()
    {
        switch (Inner)
        {
            case Bytes bytes:
                return bytes.Uri;
            case Uri uri:
                return uri.Value;
            default:
                return null;
        }
    }
}