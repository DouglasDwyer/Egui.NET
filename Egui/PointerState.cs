using System.Collections.Immutable;

namespace Egui
{

    public partial record struct PointerState
    {
    }
}

namespace Egui.Util
{
    /// <summary>
    /// Holds the history of the pointer.
    /// </summary>
    internal struct History
    {
        private ulong _minLen;
        private ulong _maxLen;
        private float _maxAge;
        private ulong _totalCount;
        private ImmutableArray<(double, EPos2)> _values;

        internal static void Serialize(BincodeSerializer serializer, History value) => value.Serialize(serializer);

        internal void Serialize(BincodeSerializer serializer)
        {
            serializer.serialize_u64(_minLen);
            serializer.serialize_u64(_maxLen);
            serializer.serialize_f32(_maxAge);
            serializer.serialize_u64(_totalCount);
            serialize_vector_tuple_double_EPos2(_values, serializer);
        }

        internal static History Deserialize(BincodeDeserializer deserializer)
        {
            History obj = default;
            obj._minLen = deserializer.deserialize_u64();
            obj._maxLen = deserializer.deserialize_u64();
            obj._maxAge = deserializer.deserialize_f32();
            obj._totalCount = deserializer.deserialize_u64();
            obj._values = deserialize_vector_tuple_double_EPos2(deserializer);
            return obj;
        }

        public override bool Equals(object? obj) => obj is History other && Equals(other);

        public static bool operator ==(History left, History right) => Equals(left, right);

        public static bool operator !=(History left, History right) => !Equals(left, right);

        public bool Equals(History other)
        {
            return _minLen == other._minLen
                && _maxLen == other._maxLen
                && _maxAge == other._maxAge
                && _values.SequenceEqual(other._values);
        }

        public override int GetHashCode()
        {
            return _values.GetHashCode();
        }

        private static ImmutableArray<(double, EPos2)> deserialize_vector_tuple_double_EPos2(Bincode.BincodeDeserializer deserializer)
        {
            return deserializer.deserialize_seq_unmanaged<(double, EPos2)>();
        }

        private static void serialize_vector_tuple_double_EPos2(ImmutableArray<(double, EPos2)> value, Bincode.BincodeSerializer serializer)
        {
            serializer.serialize_seq_unmanaged(value.AsSpan());
        }
    }
}