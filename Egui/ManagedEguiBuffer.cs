namespace Egui;

/// <summary>
/// An owner of a Rust-allocated growable byte buffer (<c>EguiBuffer</c>) to which <c>egui</c> writes call results.
/// </summary>
/// <remarks>
/// A buffer may only be used by one in-flight call at a time. Every call that writes to it
/// invalidates previously-obtained spans.
/// </remarks>
internal sealed unsafe class ManagedEguiBuffer : IDisposable
{
    /// <summary>
    /// The underlying Rust buffer, or null once disposed.
    /// </summary>
    private EguiBuffer* _handle;

    /// <summary>
    /// Allocates a new, empty buffer.
    /// </summary>
    public ManagedEguiBuffer()
    {
        _handle = EguiBindings.egui_buffer_new();
    }

    /// <summary>
    /// Frees the buffer if it was never disposed.
    /// </summary>
    ~ManagedEguiBuffer()
    {
        Free();
    }

    /// <summary>
    /// The underlying Rust buffer, to pass to native calls.
    /// </summary>
    public EguiBuffer* Handle => _handle != null ? _handle : throw new ObjectDisposedException(nameof(ManagedEguiBuffer));

    /// <summary>
    /// Gets the current contents of the buffer. The span is only valid until
    /// the next native call that writes to this buffer.
    /// </summary>
    public ReadOnlySpan<byte> AsReadOnlySpan()
    {
        var slice = EguiBindings.egui_buffer_data(Handle);
        return new ReadOnlySpan<byte>(slice.ptr, checked((int)slice.len));
    }

    /// <summary>
    /// Frees the buffer.
    /// </summary>
    public void Dispose()
    {
        Free();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Frees the underlying Rust buffer, if it has not been freed already.
    /// </summary>
    private void Free()
    {
        if (_handle != null)
        {
            EguiBindings.egui_buffer_drop(_handle);
            _handle = null;
        }
    }
}
