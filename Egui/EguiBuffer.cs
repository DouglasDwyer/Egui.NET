namespace Egui;

/// <summary>
/// A growable byte buffer owned by Rust (a <c>Vec&lt;u8&gt;</c>) to which <c>egui</c> writes call results.
/// Can be read as a <see cref="ReadOnlySpan{T}"/> or, for deserialization, as a read-only <see cref="Stream"/>.
/// </summary>
/// <remarks>
/// A buffer may only be used by one in-flight call at a time. Every call that writes to it
/// invalidates previously-read spans, so callers must <see cref="Refresh"/> afterwards.
/// </remarks>
internal sealed unsafe class EguiBuffer : Stream
{
    /// <summary>
    /// The underlying Rust <c>Vec&lt;u8&gt;</c>, or null once disposed.
    /// </summary>
    private EguiByteVec* _handle;

    /// <summary>
    /// The start of the buffer contents, as of the last <see cref="Refresh"/>.
    /// </summary>
    private byte* _data;

    /// <summary>
    /// The length of the buffer contents, as of the last <see cref="Refresh"/>.
    /// </summary>
    private int _length;

    /// <summary>
    /// The current read position.
    /// </summary>
    private int _position;

    /// <summary>
    /// Allocates a new, empty buffer.
    /// </summary>
    public EguiBuffer()
    {
        _handle = EguiBindings.egui_buffer_new();
    }

    /// <summary>
    /// Frees the buffer if it was never disposed.
    /// </summary>
    ~EguiBuffer()
    {
        Dispose(false);
    }

    /// <summary>
    /// The underlying Rust buffer, to pass to native calls.
    /// </summary>
    public EguiByteVec* Handle => _handle != null ? _handle : throw new ObjectDisposedException(nameof(EguiBuffer));

    /// <summary>
    /// Re-reads the buffer's location and length from Rust and rewinds to the start.
    /// Must be called after anything writes to the buffer.
    /// </summary>
    public void Refresh()
    {
        var slice = EguiBindings.egui_buffer_data(Handle);
        _data = slice.ptr;
        _length = checked((int)slice.len);
        _position = 0;
    }

    /// <summary>
    /// Gets the buffer contents as of the last <see cref="Refresh"/>.
    /// </summary>
    public ReadOnlySpan<byte> AsSpan() => new ReadOnlySpan<byte>(_data, _length);

    /// <inheritdoc/>
    public override bool CanRead => true;

    /// <inheritdoc/>
    public override bool CanSeek => true;

    /// <inheritdoc/>
    public override bool CanWrite => false;

    /// <inheritdoc/>
    public override long Length => _length;

    /// <inheritdoc/>
    public override long Position
    {
        get => _position;
        set => _position = value >= 0 && value <= int.MaxValue ? (int)value : throw new ArgumentOutOfRangeException(nameof(value));
    }

    /// <inheritdoc/>
    public override int Read(byte[] buffer, int offset, int count) => Read(buffer.AsSpan(offset, count));

    /// <inheritdoc/>
    public override int Read(Span<byte> buffer)
    {
        var count = Math.Min(buffer.Length, Math.Max(0, _length - _position));
        AsSpan().Slice(_position, count).CopyTo(buffer);
        _position += count;
        return count;
    }

    /// <inheritdoc/>
    public override int ReadByte() => _position < _length ? _data[_position++] : -1;

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin)
    {
        Position = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => _position + offset,
            _ => _length + offset,
        };
        return _position;
    }

    /// <inheritdoc/>
    public override void Flush() { }

    /// <inheritdoc/>
    public override void SetLength(long value) => throw new NotSupportedException();

    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (_handle != null)
        {
            EguiBindings.egui_buffer_drop(_handle);
            _handle = null;
            _data = null;
            _length = 0;
        }

        base.Dispose(disposing);
    }
}
