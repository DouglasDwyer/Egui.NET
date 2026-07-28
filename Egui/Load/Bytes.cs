using System.Collections.Immutable;

namespace Egui.Load;

public partial record struct Bytes
{
    /// <summary>
    /// Creates a new bytes object for the given data.
    /// </summary>
    public Bytes(ImmutableArray<byte> value)
    {
        _value = value;
    }
}