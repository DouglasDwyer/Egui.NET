using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Bincode;
using Serde;

namespace Egui;

/// <summary>
/// Manages C#-Rust interop and facilitates calling <c>egui</c> functions.
/// </summary>
internal static class EguiMarshal
{
    /// <summary>
    /// The serializer to use for temporary operations.
    /// </summary>
    [ThreadStatic]
    private static BincodeSerializer? _serializer;

    /// <summary>
    /// The deserializer to use for temporary operations.
    /// </summary>
    [ThreadStatic]
    private static BincodeDeserializer? _deserializer;

    /// <summary>
    /// The stream to provide to the deserializer.
    /// </summary>
    [ThreadStatic]
    private static EguiResultStream? _deserializerStream;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Call(EguiFn func)
    {
        Call<NoArgument>(func, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Call<R>(EguiFn func)
    {
        return Call<NoArgument, R>(func, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Call<A0>(EguiFn func, A0 arg0)
    {
        Call<A0, NoArgument>(func, arg0, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Call<A0, R>(EguiFn func, A0 arg0)
    {
        return Call<A0, NoArgument, R>(func, arg0, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Call<A0, A1>(EguiFn func, A0 arg0, A1 arg1)
    {
        Call<A0, A1, NoArgument>(func, arg0, arg1, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Call<A0, A1, R>(EguiFn func, A0 arg0, A1 arg1)
    {
        return Call<A0, A1, NoArgument, R>(func, arg0, arg1, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Call<A0, A1, A2>(EguiFn func, A0 arg0, A1 arg1, A2 arg2)
    {
        Call<A0, A1, A2, NoArgument>(func, arg0, arg1, arg2, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Call<A0, A1, A2, R>(EguiFn func, A0 arg0, A1 arg1, A2 arg2)
    {
        return Call<A0, A1, A2, NoArgument, R>(func, arg0, arg1, arg2, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Call<A0, A1, A2, A3>(EguiFn func, A0 arg0, A1 arg1, A2 arg2, A3 arg3)
    {
        Call<A0, A1, A2, A3, NoArgument>(func, arg0, arg1, arg2, arg3, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Call<A0, A1, A2, A3, R>(EguiFn func, A0 arg0, A1 arg1, A2 arg2, A3 arg3)
    {
        return Call<A0, A1, A2, A3, NoArgument, R>(func, arg0, arg1, arg2, arg3, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Call<A0, A1, A2, A3, A4>(EguiFn func, A0 arg0, A1 arg1, A2 arg2, A3 arg3, A4 arg4)
    {
        Call<A0, A1, A2, A3, A4, NoArgument>(func, arg0, arg1, arg2, arg3, arg4, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Call<A0, A1, A2, A3, A4, R>(EguiFn func, A0 arg0, A1 arg1, A2 arg2, A3 arg3, A4 arg4)
    {
        return Call<A0, A1, A2, A3, A4, NoArgument, R>(func, arg0, arg1, arg2, arg3, arg4, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Call<A0, A1, A2, A3, A4, A5>(EguiFn func, A0 arg0, A1 arg1, A2 arg2, A3 arg3, A4 arg4, A5 arg5)
    {
        unsafe
        {
            var serializer = GetSerializer();
            SerializerCache<A0>.Serialize(serializer, arg0);
            SerializerCache<A1>.Serialize(serializer, arg1);
            SerializerCache<A2>.Serialize(serializer, arg2);
            SerializerCache<A3>.Serialize(serializer, arg3);
            SerializerCache<A4>.Serialize(serializer, arg4);
            SerializerCache<A5>.Serialize(serializer, arg5);

            var bytes = serializer.get_bytes();
            fixed (byte* bytePtr = bytes)
            {
                var result = EguiBindings.egui_invoke(func, new EguiSliceU8
                {
                    ptr = bytePtr,
                    len = (nuint)bytes.Length
                });

                AssertSuccess(result);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Call<A0, A1, A2, A3, A4, A5, R>(EguiFn func, A0 arg0, A1 arg1, A2 arg2, A3 arg3, A4 arg4, A5 arg5)
    {
        unsafe
        {
            var serializer = GetSerializer();
            SerializerCache<A0>.Serialize(serializer, arg0);
            SerializerCache<A1>.Serialize(serializer, arg1);
            SerializerCache<A2>.Serialize(serializer, arg2);
            SerializerCache<A3>.Serialize(serializer, arg3);
            SerializerCache<A4>.Serialize(serializer, arg4);
            SerializerCache<A5>.Serialize(serializer, arg5);

            var bytes = serializer.get_bytes();
            fixed (byte* bytePtr = bytes)
            {
                var result = EguiBindings.egui_invoke(func, new EguiSliceU8
                {
                    ptr = bytePtr,
                    len = (nuint)bytes.Length
                });

                return DeserializeResult<R>(result);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe static R DeserializeResult<R>(EguiInvokeResult result)
    {
        AssertSuccess(result);
        var deserializer = GetDeserializer(result.return_value);
        return SerializerCache<R>.Deserialize(deserializer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal unsafe static void AssertSuccess(EguiInvokeResult result)
    {
        if (!result.success)
        {
            throw new EguiException(new string((char*)result.return_value.ptr, 0, (int)result.return_value.len / sizeof(char)));
        }
    }

    /// <summary>
    /// Obtains a serializer to use for temporary operations.
    /// The returned object is only valid until the next call to this function
    /// (because the underlying buffer is reused).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static BincodeSerializer GetSerializer()
    {
        if (_serializer is null)
        {
            _serializer = new BincodeSerializer();
        }

        _serializer.Reset();
        return _serializer;
    }

    /// <summary>
    /// Obtains a deserializer to use for the given result data.
    /// </summary>
    /// <remarks>Safety: the deserializer can only be safely used while the <paramref name="resultData"/> buffer is valid,
    /// and until this function is called again (because the underlying buffer is reused).</remarks>
    /// <param name="resultData">The result that was returned.</param>
    /// <returns>The deserializer.</returns>
    private unsafe static BincodeDeserializer GetDeserializer(EguiSliceU8 resultData)
    {
        if (_deserializer is null)
        {
            _deserializerStream = new EguiResultStream();
            _deserializer = new BincodeDeserializer(_deserializerStream);
        }

        _deserializerStream!.Initialize(resultData);
        return _deserializer;
    }

    /// <summary>
    /// Serializers for generic types.
    /// </summary>
    private static Dictionary<Type, (string, string)> SerializerPrototypes = new Dictionary<Type, (string, string)> {
        { typeof(ImmutableArray<>), (nameof(ImmutableArraySerializer), nameof(ImmutableArrayDeserializer)) },
        { typeof(ValueTuple<,>), (nameof(Tuple2Serializer), nameof(Tuple2Deserializer)) },
        { typeof(ValueTuple<,,>), (nameof(Tuple3Serializer), nameof(Tuple3Deserializer)) },
        { typeof(ValueTuple<,,,>), (nameof(Tuple4Serializer), nameof(Tuple4Deserializer)) },
        { typeof(Nullable<>), (nameof(NullableSerializer), nameof(NullableDeserializer)) },
    };

    /// <summary>
    /// Caches serialization and deserialization methods for a type.
    /// </summary>
    /// <typeparam name="T">The type to cache.</typeparam>
    internal static class SerializerCache<T>
    {
        /// <summary>
        /// The serialization function to use.
        /// </summary>
        public static readonly Action<BincodeSerializer, T> Serialize;

        /// <summary>
        /// The deserialization function to use.
        /// </summary>
        public static readonly Func<BincodeDeserializer, T> Deserialize;

        /// <summary>
        /// Initializes the serialization methods.
        /// </summary>
        static SerializerCache()
        {
            if (typeof(T) == typeof(NoArgument))
            {
                Serialize = (_, _) => { };
                Deserialize = _ => default!;
            }
            else if (typeof(T).IsEnum)
            {
                Serialize = (serializer, value) =>
                {
                    serializer.increase_container_depth();
                    serializer.serialize_variant_index((int)(object)value!);
                    serializer.decrease_container_depth();
                };
                Deserialize = deserializer =>
                {
                    deserializer.increase_container_depth();
                    int index = deserializer.deserialize_variant_index();
                    if (!Enum.IsDefined(typeof(T), index))
                    {
                        throw new EguiException($"Unknown variant index for {typeof(T)}: {index}");
                    }

                    deserializer.decrease_container_depth();
                    return (T)(object)index;
                };
            }
            else if (typeof(T) == typeof(string))
            {
                Serialize = (serializer, value) => serializer.serialize_str((string)(object)value!);
                Deserialize = deserializer => (T)(object)deserializer.deserialize_str();
            }
            else if (typeof(T) == typeof(bool))
            {
                Serialize = (serializer, value) => serializer.serialize_bool((bool)(object)value!);
                Deserialize = deserializer => (T)(object)deserializer.deserialize_bool();
            }
            else if (typeof(T) == typeof(Rune))
            {
                Serialize = (serializer, value) => serializer.serialize_rune((Rune)(object)value!);
                Deserialize = deserializer => (T)(object)deserializer.deserialize_rune();
            }
            else if (typeof(T) == typeof(byte))
            {
                Serialize = (serializer, value) => serializer.serialize_u8((byte)(object)value!);
                Deserialize = deserializer => (T)(object)deserializer.deserialize_u8();
            }
            else if (typeof(T) == typeof(ushort))
            {
                Serialize = (serializer, value) => serializer.serialize_u16((ushort)(object)value!);
                Deserialize = deserializer => (T)(object)deserializer.deserialize_u16();
            }
            else if (typeof(T) == typeof(uint))
            {
                Serialize = (serializer, value) => serializer.serialize_u32((uint)(object)value!);
                Deserialize = deserializer => (T)(object)deserializer.deserialize_u32();
            }
            else if (typeof(T) == typeof(ulong))
            {
                Serialize = (serializer, value) => serializer.serialize_u64((ulong)(object)value!);
                Deserialize = deserializer => (T)(object)deserializer.deserialize_u64();
            }
            else if (typeof(T) == typeof(nuint))
            {
                Serialize = (serializer, value) => serializer.serialize_u64((nuint)(object)value!);
                Deserialize = deserializer => (T)(object)(nuint)deserializer.deserialize_u64();
            }
            else if (typeof(T) == typeof(UInt128))
            {
                Serialize = (serializer, value) => serializer.serialize_u128((UInt128)(object)value!);
                Deserialize = deserializer => (T)(object)deserializer.deserialize_u128();
            }
            else if (typeof(T) == typeof(sbyte))
            {
                Serialize = (serializer, value) => serializer.serialize_i8((sbyte)(object)value!);
                Deserialize = deserializer => (T)(object)deserializer.deserialize_i8();
            }
            else if (typeof(T) == typeof(short))
            {
                Serialize = (serializer, value) => serializer.serialize_i16((short)(object)value!);
                Deserialize = deserializer => (T)(object)deserializer.deserialize_i16();
            }
            else if (typeof(T) == typeof(int))
            {
                Serialize = (serializer, value) => serializer.serialize_i32((int)(object)value!);
                Deserialize = deserializer => (T)(object)deserializer.deserialize_i32();
            }
            else if (typeof(T) == typeof(long))
            {
                Serialize = (serializer, value) => serializer.serialize_i64((long)(object)value!);
                Deserialize = deserializer => (T)(object)deserializer.deserialize_i64();
            }
            else if (typeof(T) == typeof(nint))
            {
                Serialize = (serializer, value) => serializer.serialize_i64((nint)(object)value!);
                Deserialize = deserializer => (T)(object)(nint)deserializer.deserialize_i64();
            }
            else if (typeof(T) == typeof(Int128))
            {
                Serialize = (serializer, value) => serializer.serialize_i128((Int128)(object)value!);
                Deserialize = deserializer => (T)(object)deserializer.deserialize_i128();
            }
            else if (typeof(T) == typeof(float))
            {
                Serialize = (serializer, value) => serializer.serialize_f32((float)(object)value!);
                Deserialize = deserializer => (T)(object)deserializer.deserialize_f32();
            }
            else if (typeof(T) == typeof(double))
            {
                Serialize = (serializer, value) => serializer.serialize_f64((double)(object)value!);
                Deserialize = deserializer => (T)(object)deserializer.deserialize_f64();
            }
            else if (typeof(T).IsGenericType && SerializerPrototypes.TryGetValue(typeof(T).GetGenericTypeDefinition(), out var methods))
            {
                var genericArgs = typeof(T).GenericTypeArguments;
                var serializer = typeof(EguiMarshal).GetMethod(methods.Item1, BindingFlags.NonPublic | BindingFlags.Static)!.MakeGenericMethod(genericArgs);
                var deserializer = typeof(EguiMarshal).GetMethod(methods.Item2, BindingFlags.NonPublic | BindingFlags.Static)!.MakeGenericMethod(genericArgs);

                Serialize = (Action<BincodeSerializer, T>)Delegate.CreateDelegate(typeof(Action<BincodeSerializer, T>), serializer);
                Deserialize = (Func<BincodeDeserializer, T>)Delegate.CreateDelegate(typeof(Func<BincodeDeserializer, T>), deserializer);
            }
            else
            {
                var serializer = typeof(T).GetMethod("Serialize", BindingFlags.Static | BindingFlags.NonPublic)!;
                var deserializer = typeof(T).GetMethod("Deserialize", BindingFlags.Static | BindingFlags.NonPublic)!;

                if (serializer == null || deserializer == null)
                {
                    throw new EguiException($"Missing serializers for {typeof(T)}");
                }
                Serialize = (Action<BincodeSerializer, T>)Delegate.CreateDelegate(typeof(Action<BincodeSerializer, T>), serializer);
                Deserialize = (Func<BincodeDeserializer, T>)Delegate.CreateDelegate(typeof(Func<BincodeDeserializer, T>), deserializer);
            }
        }
    }

    /// <summary>
    /// Serializes an immutable array.
    /// </summary>
    private static void ImmutableArraySerializer<T>(BincodeSerializer serializer, ImmutableArray<T> value)
    {
        serializer.serialize_len(value.Length);
        foreach (var item in value)
        {
            SerializerCache<T>.Serialize(serializer, item);
        }
    }

    /// <summary>
    /// Deserializes an immutable array.
    /// </summary>
    private static ImmutableArray<T> ImmutableArrayDeserializer<T>(BincodeDeserializer deserializer)
    {
        var length = deserializer.deserialize_len();
        T[] obj = new T[length];
        for (int i = 0; i < length; i++)
        {
            obj[i] = SerializerCache<T>.Deserialize(deserializer);
        }
        return obj.ToImmutableArray();
    }

    /// <summary>
    /// Serializes a tuple.
    /// </summary>
    private static void Tuple2Serializer<A0, A1>(BincodeSerializer serializer, (A0, A1) value)
    {
        SerializerCache<A0>.Serialize(serializer, value.Item1);
        SerializerCache<A1>.Serialize(serializer, value.Item2);
    }

    /// <summary>
    /// Deserializes a tuple.
    /// </summary>
    private static (A0, A1) Tuple2Deserializer<A0, A1>(BincodeDeserializer deserializer)
    {
        return (SerializerCache<A0>.Deserialize(deserializer), SerializerCache<A1>.Deserialize(deserializer));
    }

    /// <summary>
    /// Serializes a tuple.
    /// </summary>
    private static void Tuple3Serializer<A0, A1, A2>(BincodeSerializer serializer, (A0, A1, A2) value)
    {
        SerializerCache<A0>.Serialize(serializer, value.Item1);
        SerializerCache<A1>.Serialize(serializer, value.Item2);
        SerializerCache<A2>.Serialize(serializer, value.Item3);
    }

    /// <summary>
    /// Deserializes a tuple.
    /// </summary>
    private static (A0, A1, A2) Tuple3Deserializer<A0, A1, A2>(BincodeDeserializer deserializer)
    {
        return (
            SerializerCache<A0>.Deserialize(deserializer),
            SerializerCache<A1>.Deserialize(deserializer),
            SerializerCache<A2>.Deserialize(deserializer)
        );
    }

    /// <summary>
    /// Serializes a tuple.
    /// </summary>
    private static void Tuple4Serializer<A0, A1, A2, A3>(BincodeSerializer serializer, (A0, A1, A2, A3) value)
    {
        SerializerCache<A0>.Serialize(serializer, value.Item1);
        SerializerCache<A1>.Serialize(serializer, value.Item2);
        SerializerCache<A2>.Serialize(serializer, value.Item3);
        SerializerCache<A3>.Serialize(serializer, value.Item4);
    }

    /// <summary>
    /// Deserializes a tuple.
    /// </summary>
    private static (A0, A1, A2, A3) Tuple4Deserializer<A0, A1, A2, A3>(BincodeDeserializer deserializer)
    {
        return (
            SerializerCache<A0>.Deserialize(deserializer),
            SerializerCache<A1>.Deserialize(deserializer),
            SerializerCache<A2>.Deserialize(deserializer),
            SerializerCache<A3>.Deserialize(deserializer)
        );
    }

    /// <summary>
    /// Serializes a nullable.
    /// </summary>
    private static void NullableSerializer<T>(BincodeSerializer serializer, T? value) where T : struct
    {
        serializer.serialize_option_tag(value.HasValue);
        if (value.HasValue)
        {
            SerializerCache<T>.Serialize(serializer, value.Value);
        }
    }

    /// <summary>
    /// Deserializes a nullable.
    /// </summary>
    private static T? NullableDeserializer<T>(BincodeDeserializer deserializer) where T : struct
    {
        if (deserializer.deserialize_option_tag())
        {
            return SerializerCache<T>.Deserialize(deserializer);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Marker struct indicating that this is an extra argument.
    /// </summary>
    private struct NoArgument
    {

    }

    /// <summary>
    /// Allows for reading <c>egui</c> result data from unmanaged memory.
    /// </summary>
    private unsafe sealed class EguiResultStream : UnmanagedMemoryStream
    {
        /// <summary>
        /// Returns true if the stream can be read; otherwise returns false.
        /// </summary>
        public override bool CanRead => true;

        /// <summary>
        /// Returns true if the stream can seek; otherwise returns false.
        /// </summary>
        public override bool CanSeek => true;
        
        /// <summary>
        /// Creates a new, uninitialized stream.
        /// </summary>
        public EguiResultStream() { }

        /// <summary>
        /// Sets the buffer referenced by the stream.
        /// </summary>
        /// <param name="slice">The buffer to use.</param>
        public void Initialize(EguiSliceU8 slice)
        {
            Dispose(true);
            Initialize(slice.ptr, (long)slice.len, (long)slice.len, FileAccess.Read);
            Position = 0;
        }
    }
}