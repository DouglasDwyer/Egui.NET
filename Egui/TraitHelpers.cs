using System.Collections.Immutable;
using Egui.Plot;

namespace Egui;

/// <summary>
/// Method definitions for serialization.
/// </summary>
static partial class TraitHelpers
{
    public static void serialize_option_UiStack(UiStack? value, BincodeSerializer serializer)
    {
        if (value is not null)
        {
            serializer.serialize_option_tag(true);
            (value ?? default).Serialize(serializer);
        }
        else
        {
            serializer.serialize_option_tag(false);
        }
    }

    public static UiStack? deserialize_option_UiStack(BincodeDeserializer deserializer)
    {
        bool tag = deserializer.deserialize_option_tag();
        if (!tag)
        {
            return null;
        }
        else
        {
            return UiStack.Deserialize(deserializer);
        }
    }

    public static void serialize_option_LineStyle(LineStyle? value, BincodeSerializer serializer)
    {
        if (value is not null)
        {
            serializer.serialize_option_tag(true);
            (value ?? default).Serialize(serializer);
        }
        else
        {
            serializer.serialize_option_tag(false);
        }
    }

    public static LineStyle? deserialize_option_LineStyle(BincodeDeserializer deserializer)
    {
        bool tag = deserializer.deserialize_option_tag();
        if (!tag) { return null; }
        return LineStyle.Deserialize(deserializer);
    }

    public static void serialize_option_MarkerShape(MarkerShape? value, BincodeSerializer serializer)
    {
        if (value is not null)
        {
            serializer.serialize_option_tag(true);
            (value ?? default).Serialize(serializer);
        }
        else
        {
            serializer.serialize_option_tag(false);
        }
    }

    public static MarkerShape? deserialize_option_MarkerShape(BincodeDeserializer deserializer)
    {
        bool tag = deserializer.deserialize_option_tag();
        if (!tag) { return null; }
        return MarkerShapeSerdeExtensions.Deserialize(deserializer);
    }

    public static void serialize_vector_Color32(ImmutableArray<Egui.Color32> value, Bincode.BincodeSerializer serializer)
    {
        serializer.serialize_seq_unmanaged(value.AsSpan());
    }

    public static ImmutableArray<Egui.Color32> deserialize_vector_Color32(Bincode.BincodeDeserializer deserializer)
    {
        return deserializer.deserialize_seq_unmanaged<Color32>();
    }

    public static void serialize_vector_Vertex(ImmutableArray<Egui.Epaint.Vertex> value, Bincode.BincodeSerializer serializer)
    {
        serializer.serialize_seq_unmanaged(value.AsSpan());
    }

    public static ImmutableArray<Egui.Epaint.Vertex> deserialize_vector_Vertex(Bincode.BincodeDeserializer deserializer)
    {
        return deserializer.deserialize_seq_unmanaged<Vertex>();
    }

    public static void serialize_vector_u8(ImmutableArray<byte> value, Bincode.BincodeSerializer serializer)
    {
        serializer.serialize_seq_unmanaged(value.AsSpan());
    }

    public static ImmutableArray<byte> deserialize_vector_u8(Bincode.BincodeDeserializer deserializer)
    {
        return deserializer.deserialize_seq_unmanaged<byte>();
    }

    public static void serialize_vector_u16(ImmutableArray<ushort> value, Bincode.BincodeSerializer serializer)
    {
        serializer.serialize_seq_unmanaged(value.AsSpan());
    }

    public static ImmutableArray<ushort> deserialize_vector_u16(Bincode.BincodeDeserializer deserializer)
    {
        return deserializer.deserialize_seq_unmanaged<ushort>();
    }

    public static void serialize_vector_u32(ImmutableArray<uint> value, Bincode.BincodeSerializer serializer)
    {
        serializer.serialize_seq_unmanaged(value.AsSpan());
    }

    public static ImmutableArray<uint> deserialize_vector_u32(Bincode.BincodeDeserializer deserializer)
    {
        return deserializer.deserialize_seq_unmanaged<uint>();
    }

    public static void serialize_vector_u64(ImmutableArray<ulong> value, Bincode.BincodeSerializer serializer)
    {
        serializer.serialize_seq_unmanaged(value.AsSpan());
    }

    public static ImmutableArray<ulong> deserialize_vector_u64(Bincode.BincodeDeserializer deserializer)
    {
        return deserializer.deserialize_seq_unmanaged<ulong>();
    }
}