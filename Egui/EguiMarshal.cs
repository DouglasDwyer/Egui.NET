using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
    /// The call state reused by non-re-entrant calls on this thread.
    /// </summary>
    [ThreadStatic]
    private static CallState? _cachedState;

    /// <summary>
    /// Whether <see cref="_cachedState"/> is currently held by an in-flight call.
    /// </summary>
    [ThreadStatic]
    private static bool _cachedStateInUse;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Call(EguiFn func)
    {
        Call<NoArgument>(func, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Call<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] R>(EguiFn func)
    {
        return Call<NoArgument, R>(func, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Call<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A0>(EguiFn func, A0 arg0)
    {
        Call<A0, NoArgument>(func, arg0, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Call<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A0, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] R>(EguiFn func, A0 arg0)
    {
        return Call<A0, NoArgument, R>(func, arg0, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Call<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A0, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A1>(EguiFn func, A0 arg0, A1 arg1)
    {
        Call<A0, A1, NoArgument>(func, arg0, arg1, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Call<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A0, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A1, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] R>(EguiFn func, A0 arg0, A1 arg1)
    {
        return Call<A0, A1, NoArgument, R>(func, arg0, arg1, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Call<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A0, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A1, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A2>(EguiFn func, A0 arg0, A1 arg1, A2 arg2)
    {
        Call<A0, A1, A2, NoArgument>(func, arg0, arg1, arg2, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Call<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A0, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A1, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A2, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] R>(EguiFn func, A0 arg0, A1 arg1, A2 arg2)
    {
        return Call<A0, A1, A2, NoArgument, R>(func, arg0, arg1, arg2, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Call<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A0, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A1, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A2, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A3>(EguiFn func, A0 arg0, A1 arg1, A2 arg2, A3 arg3)
    {
        Call<A0, A1, A2, A3, NoArgument>(func, arg0, arg1, arg2, arg3, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Call<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A0, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A1, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A2, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A3, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] R>(EguiFn func, A0 arg0, A1 arg1, A2 arg2, A3 arg3)
    {
        return Call<A0, A1, A2, A3, NoArgument, R>(func, arg0, arg1, arg2, arg3, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Call<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A0, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A1, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A2, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A3, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A4>(EguiFn func, A0 arg0, A1 arg1, A2 arg2, A3 arg3, A4 arg4)
    {
        Call<A0, A1, A2, A3, A4, NoArgument>(func, arg0, arg1, arg2, arg3, arg4, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Call<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A0, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A1, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A2, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A3, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A4, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] R>(EguiFn func, A0 arg0, A1 arg1, A2 arg2, A3 arg3, A4 arg4)
    {
        return Call<A0, A1, A2, A3, A4, NoArgument, R>(func, arg0, arg1, arg2, arg3, arg4, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Call<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A0, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A1, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A2, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A3, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A4, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A5>(EguiFn func, A0 arg0, A1 arg1, A2 arg2, A3 arg3, A4 arg4, A5 arg5)
    {
        var state = AcquireState();
        try
        {
                SerializerCache<A0>.Serialize(state.Serializer, arg0);
                SerializerCache<A1>.Serialize(state.Serializer, arg1);
                SerializerCache<A2>.Serialize(state.Serializer, arg2);
                SerializerCache<A3>.Serialize(state.Serializer, arg3);
                SerializerCache<A4>.Serialize(state.Serializer, arg4);
                SerializerCache<A5>.Serialize(state.Serializer, arg5);

            Invoke(func, state);
            AssertSuccess(state);
        }
        finally
        {
            ReleaseState(state);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Call<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A0, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A1, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A2, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A3, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A4, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A5, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] R>(EguiFn func, A0 arg0, A1 arg1, A2 arg2, A3 arg3, A4 arg4, A5 arg5)
    {
        var state = AcquireState();
        try
        {
                SerializerCache<A0>.Serialize(state.Serializer, arg0);
                SerializerCache<A1>.Serialize(state.Serializer, arg1);
                SerializerCache<A2>.Serialize(state.Serializer, arg2);
                SerializerCache<A3>.Serialize(state.Serializer, arg3);
                SerializerCache<A4>.Serialize(state.Serializer, arg4);
                SerializerCache<A5>.Serialize(state.Serializer, arg5);

            Invoke(func, state);
            return DeserializeResult<R>(state);
        }
        finally
        {
            ReleaseState(state);
        }
    }

    /// <summary>
    /// Sends the serialized arguments in <paramref name="state"/> to Rust and
    /// stores the result in its buffer.
    /// </summary>
    private unsafe static void Invoke(EguiFn func, CallState state)
    {
        var bytes = state.Serializer.get_bytes();
        bool success;
        fixed (byte* bytePtr = bytes)
        {
            success = EguiBindings.egui_invoke(func, new EguiSliceU8
            {
                ptr = bytePtr,
                len = (nuint)bytes.Length
            }, state.Buffer.Handle);
        }

        state.Buffer.Refresh();
        state.Success = success;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static R DeserializeResult<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] R>(CallState state)
    {
        AssertSuccess(state);
        return SerializerCache<R>.Deserialize(state.Deserializer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AssertSuccess(CallState state)
    {
        if (!state.Success)
        {
            throw new EguiException(new string(MemoryMarshal.Cast<byte, char>(state.Buffer.AsSpan())));
        }
    }

    /// <summary>
    /// Obtains the state for a call. The thread's cached state is reused unless
    /// it is already held by an enclosing call on this thread (for example, when a
    /// type initializer or Rust callback makes a nested call mid-serialization or
    /// mid-deserialization), in which case a fresh state is allocated.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CallState AcquireState()
    {
        CallState state;
        if (_cachedStateInUse)
        {
            state = new CallState();
        }
        else
        {
            state = _cachedState ??= new CallState();
            _cachedStateInUse = true;
        }

        state.Serializer.Reset();
        return state;
    }

    /// <summary>
    /// Releases a state obtained from <see cref="AcquireState"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ReleaseState(CallState state)
    {
        if (ReferenceEquals(state, _cachedState))
        {
            _cachedStateInUse = false;
        }
        else
        {
            state.Dispose();
        }
    }

    /// <summary>
    /// Serializers for generic types. <c>Egui.SourceGenerators</c> reads these <c>typeof(...)</c>
    /// keys directly to know which shapes need rooting for Native AOT.
    /// </summary>
    private static Dictionary<Type, (string, string)> SerializerPrototypes = new Dictionary<Type, (string, string)> {
        { typeof(ImmutableArray<>), (nameof(ImmutableArraySerializer), nameof(ImmutableArrayDeserializer)) },
        { typeof(ValueTuple<,>), (nameof(Tuple2Serializer), nameof(Tuple2Deserializer)) },
        { typeof(ValueTuple<,,>), (nameof(Tuple3Serializer), nameof(Tuple3Deserializer)) },
        { typeof(ValueTuple<,,,>), (nameof(Tuple4Serializer), nameof(Tuple4Deserializer)) },
        { typeof(Nullable<>), (nameof(NullableSerializer), nameof(NullableDeserializer)) },
        { typeof(Array2<>), (nameof(Array2Serializer), nameof(Array2Deserializer)) },
        { typeof(Array3<>), (nameof(Array3Serializer), nameof(Array3Deserializer)) },
        { typeof(Array4<>), (nameof(Array4Serializer), nameof(Array4Deserializer)) },
        { typeof(Array5<>), (nameof(Array5Serializer), nameof(Array5Deserializer)) },
        { typeof(Array6<>), (nameof(Array6Serializer), nameof(Array6Deserializer)) },
    };

    /// <summary>
    /// Caches serialization and deserialization methods for a type.
    /// </summary>
    /// <typeparam name="T">The type to cache.</typeparam>
    internal static class SerializerCache<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>
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
        /// <remarks>
        /// The compound-type branch below uses <see cref="MethodInfo.MakeGenericMethod"/>, which Native AOT
        /// can only satisfy for instantiations rooted elsewhere - see <see cref="AotRoot"/>.
        /// </remarks>
        [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "The target method is rooted; see AotRoot.")]
        [UnconditionalSuppressMessage("Trimming", "IL2060", Justification = "The target method is rooted; see AotRoot.")]
        [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "The requested instantiation is rooted; see AotRoot.")]
        static SerializerCache()
        {
            if (typeof(T) == typeof(NoArgument))
            {
                Serialize = (_, _) => { };
                Deserialize = _ => default!;
            }
            else if (typeof(T).IsEnum && Enum.GetUnderlyingType(typeof(T)) == typeof(int))
            {
                Serialize = (serializer, value) =>
                {
                    serializer.increase_container_depth();
                    serializer.serialize_variant_index(Unsafe.BitCast<T, int>(value));
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
                    return Unsafe.BitCast<int, T>(index);
                };
            }
            else if (typeof(T).IsEnum)
            {
                // Fallback for any enum not backed by `int` - boxes, but no such type is
                // currently generated (see the branch above for the common, allocation-free path).
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
                Serialize = (serializer, value) => serializer.serialize_bool(Unsafe.BitCast<T, bool>(value));
                Deserialize = deserializer => Unsafe.BitCast<bool, T>(deserializer.deserialize_bool());
            }
            else if (typeof(T) == typeof(Rune))
            {
                Serialize = (serializer, value) => serializer.serialize_rune(Unsafe.BitCast<T, Rune>(value));
                Deserialize = deserializer => Unsafe.BitCast<Rune, T>(deserializer.deserialize_rune());
            }
            else if (typeof(T) == typeof(byte))
            {
                Serialize = (serializer, value) => serializer.serialize_u8(Unsafe.BitCast<T, byte>(value));
                Deserialize = deserializer => Unsafe.BitCast<byte, T>(deserializer.deserialize_u8());
            }
            else if (typeof(T) == typeof(ushort))
            {
                Serialize = (serializer, value) => serializer.serialize_u16(Unsafe.BitCast<T, ushort>(value));
                Deserialize = deserializer => Unsafe.BitCast<ushort, T>(deserializer.deserialize_u16());
            }
            else if (typeof(T) == typeof(uint))
            {
                Serialize = (serializer, value) => serializer.serialize_u32(Unsafe.BitCast<T, uint>(value));
                Deserialize = deserializer => Unsafe.BitCast<uint, T>(deserializer.deserialize_u32());
            }
            else if (typeof(T) == typeof(ulong))
            {
                Serialize = (serializer, value) => serializer.serialize_u64(Unsafe.BitCast<T, ulong>(value));
                Deserialize = deserializer => Unsafe.BitCast<ulong, T>(deserializer.deserialize_u64());
            }
            else if (typeof(T) == typeof(nuint))
            {
                Serialize = (serializer, value) => serializer.serialize_u64(Unsafe.BitCast<T, nuint>(value));
                Deserialize = deserializer => Unsafe.BitCast<nuint, T>((nuint)deserializer.deserialize_u64());
            }
            else if (typeof(T) == typeof(UInt128))
            {
                Serialize = (serializer, value) => serializer.serialize_u128(Unsafe.BitCast<T, UInt128>(value));
                Deserialize = deserializer => Unsafe.BitCast<UInt128, T>(deserializer.deserialize_u128());
            }
            else if (typeof(T) == typeof(sbyte))
            {
                Serialize = (serializer, value) => serializer.serialize_i8(Unsafe.BitCast<T, sbyte>(value));
                Deserialize = deserializer => Unsafe.BitCast<sbyte, T>(deserializer.deserialize_i8());
            }
            else if (typeof(T) == typeof(short))
            {
                Serialize = (serializer, value) => serializer.serialize_i16(Unsafe.BitCast<T, short>(value));
                Deserialize = deserializer => Unsafe.BitCast<short, T>(deserializer.deserialize_i16());
            }
            else if (typeof(T) == typeof(int))
            {
                Serialize = (serializer, value) => serializer.serialize_i32(Unsafe.BitCast<T, int>(value));
                Deserialize = deserializer => Unsafe.BitCast<int, T>(deserializer.deserialize_i32());
            }
            else if (typeof(T) == typeof(long))
            {
                Serialize = (serializer, value) => serializer.serialize_i64(Unsafe.BitCast<T, long>(value));
                Deserialize = deserializer => Unsafe.BitCast<long, T>(deserializer.deserialize_i64());
            }
            else if (typeof(T) == typeof(nint))
            {
                Serialize = (serializer, value) => serializer.serialize_i64(Unsafe.BitCast<T, nint>(value));
                Deserialize = deserializer => Unsafe.BitCast<nint, T>((nint)deserializer.deserialize_i64());
            }
            else if (typeof(T) == typeof(Int128))
            {
                Serialize = (serializer, value) => serializer.serialize_i128(Unsafe.BitCast<T, Int128>(value));
                Deserialize = deserializer => Unsafe.BitCast<Int128, T>(deserializer.deserialize_i128());
            }
            else if (typeof(T) == typeof(float))
            {
                Serialize = (serializer, value) => serializer.serialize_f32(Unsafe.BitCast<T, float>(value));
                Deserialize = deserializer => Unsafe.BitCast<float, T>(deserializer.deserialize_f32());
            }
            else if (typeof(T) == typeof(double))
            {
                Serialize = (serializer, value) => serializer.serialize_f64(Unsafe.BitCast<T, double>(value));
                Deserialize = deserializer => Unsafe.BitCast<double, T>(deserializer.deserialize_f64());
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
    private static void ImmutableArraySerializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>(BincodeSerializer serializer, ImmutableArray<T> value)
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
    private static ImmutableArray<T> ImmutableArrayDeserializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>(BincodeDeserializer deserializer)
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
    private static void Tuple2Serializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A0, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A1>(BincodeSerializer serializer, (A0, A1) value)
    {
        SerializerCache<A0>.Serialize(serializer, value.Item1);
        SerializerCache<A1>.Serialize(serializer, value.Item2);
    }

    /// <summary>
    /// Deserializes a tuple.
    /// </summary>
    private static (A0, A1) Tuple2Deserializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A0, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A1>(BincodeDeserializer deserializer)
    {
        return (SerializerCache<A0>.Deserialize(deserializer), SerializerCache<A1>.Deserialize(deserializer));
    }

    /// <summary>
    /// Serializes a tuple.
    /// </summary>
    private static void Tuple3Serializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A0, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A1, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A2>(BincodeSerializer serializer, (A0, A1, A2) value)
    {
        SerializerCache<A0>.Serialize(serializer, value.Item1);
        SerializerCache<A1>.Serialize(serializer, value.Item2);
        SerializerCache<A2>.Serialize(serializer, value.Item3);
    }

    /// <summary>
    /// Deserializes a tuple.
    /// </summary>
    private static (A0, A1, A2) Tuple3Deserializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A0, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A1, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A2>(BincodeDeserializer deserializer)
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
    private static void Tuple4Serializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A0, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A1, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A2, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A3>(BincodeSerializer serializer, (A0, A1, A2, A3) value)
    {
        SerializerCache<A0>.Serialize(serializer, value.Item1);
        SerializerCache<A1>.Serialize(serializer, value.Item2);
        SerializerCache<A2>.Serialize(serializer, value.Item3);
        SerializerCache<A3>.Serialize(serializer, value.Item4);
    }

    /// <summary>
    /// Deserializes a tuple.
    /// </summary>
    private static (A0, A1, A2, A3) Tuple4Deserializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A0, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A1, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A2, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A3>(BincodeDeserializer deserializer)
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
    private static void NullableSerializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>(BincodeSerializer serializer, T? value) where T : struct
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
    private static T? NullableDeserializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>(BincodeDeserializer deserializer) where T : struct
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
    /// Serializes a fixed-size array.
    /// </summary>
    private static void Array2Serializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>(BincodeSerializer serializer, Array2<T> value)
    {
        foreach (var item in value)
        {
            SerializerCache<T>.Serialize(serializer, item);
        }
    }

    /// <summary>
    /// Deserializes a fixed-size array.
    /// </summary>
    private static Array2<T> Array2Deserializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>(BincodeDeserializer deserializer)
    {
        Array2<T> result = default;
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = SerializerCache<T>.Deserialize(deserializer);
        }
        return result;
    }

    /// <summary>
    /// Serializes a fixed-size array.
    /// </summary>
    private static void Array3Serializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>(BincodeSerializer serializer, Array3<T> value)
    {
        foreach (var item in value)
        {
            SerializerCache<T>.Serialize(serializer, item);
        }
    }

    /// <summary>
    /// Deserializes a fixed-size array.
    /// </summary>
    private static Array3<T> Array3Deserializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>(BincodeDeserializer deserializer)
    {
        Array3<T> result = default;
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = SerializerCache<T>.Deserialize(deserializer);
        }
        return result;
    }

    /// <summary>
    /// Serializes a fixed-size array.
    /// </summary>
    private static void Array4Serializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>(BincodeSerializer serializer, Array4<T> value)
    {
        foreach (var item in value)
        {
            SerializerCache<T>.Serialize(serializer, item);
        }
    }

    /// <summary>
    /// Deserializes a fixed-size array.
    /// </summary>
    private static Array4<T> Array4Deserializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>(BincodeDeserializer deserializer)
    {
        Array4<T> result = default;
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = SerializerCache<T>.Deserialize(deserializer);
        }
        return result;
    }

    /// <summary>
    /// Serializes a fixed-size array.
    /// </summary>
    private static void Array5Serializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>(BincodeSerializer serializer, Array5<T> value)
    {
        foreach (var item in value)
        {
            SerializerCache<T>.Serialize(serializer, item);
        }
    }

    /// <summary>
    /// Deserializes a fixed-size array.
    /// </summary>
    private static Array5<T> Array5Deserializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>(BincodeDeserializer deserializer)
    {
        Array5<T> result = default;
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = SerializerCache<T>.Deserialize(deserializer);
        }
        return result;
    }

    /// <summary>
    /// Serializes a fixed-size array.
    /// </summary>
    private static void Array6Serializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>(BincodeSerializer serializer, Array6<T> value)
    {
        foreach (var item in value)
        {
            SerializerCache<T>.Serialize(serializer, item);
        }
    }

    /// <summary>
    /// Deserializes a fixed-size array.
    /// </summary>
    private static Array6<T> Array6Deserializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>(BincodeDeserializer deserializer)
    {
        Array6<T> result = default;
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = SerializerCache<T>.Deserialize(deserializer);
        }
        return result;
    }

    /// <summary>
    /// Roots closed generic instantiations of the compound-type serializer/deserializer pairs
    /// above, so Native AOT compiles them ahead of time instead of only discovering them
    /// through <see cref="SerializerCache{T}"/>'s reflection-based dispatch.
    /// </summary>
    /// <remarks>
    /// Not meant to be called by hand: <c>Egui.SourceGenerators</c> emits a module initializer
    /// that calls one of these methods for every closed instantiation the compilation actually
    /// uses. The reference alone is enough to root it - the methods never need to run.
    /// </remarks>
    internal static class AotRoot
    {
        internal static void ImmutableArray<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>()
        {
            _ = (Action<BincodeSerializer, ImmutableArray<T>>)ImmutableArraySerializer<T>;
            _ = (Func<BincodeDeserializer, ImmutableArray<T>>)ImmutableArrayDeserializer<T>;
        }

        internal static void Nullable<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>() where T : struct
        {
            _ = (Action<BincodeSerializer, T?>)NullableSerializer<T>;
            _ = (Func<BincodeDeserializer, T?>)NullableDeserializer<T>;
        }

        internal static void ValueTuple<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A0, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A1>()
        {
            _ = (Action<BincodeSerializer, (A0, A1)>)Tuple2Serializer<A0, A1>;
            _ = (Func<BincodeDeserializer, (A0, A1)>)Tuple2Deserializer<A0, A1>;
        }

        internal static void ValueTuple<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A0, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A1, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A2>()
        {
            _ = (Action<BincodeSerializer, (A0, A1, A2)>)Tuple3Serializer<A0, A1, A2>;
            _ = (Func<BincodeDeserializer, (A0, A1, A2)>)Tuple3Deserializer<A0, A1, A2>;
        }

        internal static void ValueTuple<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A0, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A1, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A2, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] A3>()
        {
            _ = (Action<BincodeSerializer, (A0, A1, A2, A3)>)Tuple4Serializer<A0, A1, A2, A3>;
            _ = (Func<BincodeDeserializer, (A0, A1, A2, A3)>)Tuple4Deserializer<A0, A1, A2, A3>;
        }

        internal static void Array2<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>()
        {
            _ = (Action<BincodeSerializer, Array2<T>>)Array2Serializer<T>;
            _ = (Func<BincodeDeserializer, Array2<T>>)Array2Deserializer<T>;
        }

        internal static void Array3<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>()
        {
            _ = (Action<BincodeSerializer, Array3<T>>)Array3Serializer<T>;
            _ = (Func<BincodeDeserializer, Array3<T>>)Array3Deserializer<T>;
        }

        internal static void Array4<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>()
        {
            _ = (Action<BincodeSerializer, Array4<T>>)Array4Serializer<T>;
            _ = (Func<BincodeDeserializer, Array4<T>>)Array4Deserializer<T>;
        }

        internal static void Array5<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>()
        {
            _ = (Action<BincodeSerializer, Array5<T>>)Array5Serializer<T>;
            _ = (Func<BincodeDeserializer, Array5<T>>)Array5Deserializer<T>;
        }

        internal static void Array6<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicMethods)] T>()
        {
            _ = (Action<BincodeSerializer, Array6<T>>)Array6Serializer<T>;
            _ = (Func<BincodeDeserializer, Array6<T>>)Array6Deserializer<T>;
        }
    }

    /// <summary>
    /// Marker struct indicating that this is an extra argument.
    /// </summary>
    private struct NoArgument
    {

    }

    /// <summary>
    /// The serializer, result buffer and deserializer used by a single call.
    /// </summary>
    private sealed class CallState : IDisposable
    {
        /// <summary>
        /// Serializes the call arguments.
        /// </summary>
        public readonly BincodeSerializer Serializer = new BincodeSerializer();

        /// <summary>
        /// Holds the result that Rust wrote.
        /// </summary>
        public readonly EguiBuffer Buffer = new EguiBuffer();

        /// <summary>
        /// Reads the result from <see cref="Buffer"/>.
        /// </summary>
        public readonly BincodeDeserializer Deserializer;

        /// <summary>
        /// Whether the last call succeeded. If not, <see cref="Buffer"/> holds an error message.
        /// </summary>
        public bool Success;

        public CallState()
        {
            Deserializer = new BincodeDeserializer(Buffer);
        }

        public void Dispose()
        {
            Deserializer.Dispose();
            Serializer.Dispose();
            Buffer.Dispose();
        }
    }
}