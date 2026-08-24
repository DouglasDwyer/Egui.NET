using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Egui;

public static partial class EguiHelpers
{
    /// <summary>
    /// Loads an image from an embedded resource in the calling assembly.
    /// </summary>
    public static ImageSource IncludeImageResource(string resourceName)
    {
        return IncludeImageResource(Assembly.GetCallingAssembly(), resourceName);
    }

    /// <summary>
    /// Loads an image from an embedded resource in the specified assembly.
    /// </summary>
    public static ImageSource IncludeImageResource(Assembly assembly, string resourceName)
    {
        var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            throw new FileNotFoundException($"Could not load resource {resourceName} from {assembly}");
        }

        using (var memoryStream = new MemoryStream())
        {
            stream.CopyTo(memoryStream);
            var span = MemoryMarshal.ToEnumerable(new ReadOnlyMemory<byte>(memoryStream.GetBuffer(), 0, (int)memoryStream.Length));
            return new ImageSource.Bytes
            {
                Uri = $"bytes://{resourceName}",
                Inner = new Bytes(span.ToImmutableArray())
            };
        }
    }

    /// <inheritdoc cref="ResetButtonWith{T}"/>
    public static void ResetButton<T>(Ui ui, ref T value, string text) where T : new()
    {
        ResetButtonWith(ui, ref value, text, new());
    }

    /// <summary>
    /// Show a button to reset a value to its default.
    /// The button is only enabled if the value does not already have its original value.
    ///
    /// The <paramref name="text"/> could be something like "Reset foo".
    /// </summary>
    public static void ResetButtonWith<T>(Ui ui, ref T value, string text, T resetValue)
    {
        if (ui.AddEnabled(EqualityComparer<T>.Default.Equals(value, resetValue), new Button(text)).Clicked)
        {
            value = resetValue;
        }
    }

    internal static void Serialize(this TimeSpan value, Bincode.BincodeSerializer serializer)
    {
        var totalNanos = TimeSpan.NanosecondsPerTick * value.Ticks;
        var secs = totalNanos / 1_000_000_000;
        var nanos = totalNanos % 1_000_000_000;
        serializer.increase_container_depth();
        serializer.serialize_u64((ulong)secs);
        serializer.serialize_u32((uint)nanos);
        serializer.decrease_container_depth();
    }

    internal static Duration DeserializeDuration(Bincode.BincodeDeserializer deserializer)
    {
        deserializer.increase_container_depth();
        var secs = deserializer.deserialize_u64();
        var nanos = deserializer.deserialize_u32();
        deserializer.decrease_container_depth();
        return Duration.FromTicks((long)(1_000_000_000 * secs + nanos) / TimeSpan.NanosecondsPerTick);
    }
}