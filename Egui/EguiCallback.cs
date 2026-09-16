using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Egui;

/// <summary>
/// A callback that may be passed to unmanaged code.
/// </summary>
internal unsafe partial struct EguiCallback
{
    /// <summary>
    /// Facilitates sending a callback to unmanaged code without incurring a
    /// <see cref="GCHandle"/> allocation: the callback and its exception state live inline in
    /// this stack-resident struct, and <see cref="AsNative"/> takes their address directly.
    /// </summary>
    public unsafe struct Pin : IDisposable
    {
        /// <summary>
        /// The function pointer to invoke from unmanaged code.
        /// </summary>
        private readonly delegate* unmanaged[Cdecl]<void*, void*, void> _callbackFunc;

        /// <summary>
        /// Data to pass across the FFI boundary during calls.
        /// </summary>
        private Context _context;

        /// <summary>
        /// Creates a new object for invoking the given callback.
        /// </summary>
        /// <param name="callback">The callback to invoke.</param>
        public Pin(Action<nuint> callback)
        {
            _callbackFunc = &InvokeCallback;
            _context = new Context { F = callback };
        }

        /// <summary>
        /// Gets the native object representing the callback.
        /// </summary>
        /// <returns>The native representation.</returns>
        public EguiCallback AsNative() => new EguiCallback
        {
            func = _callbackFunc,
            data = Unsafe.AsPointer(ref _context)
        };

        /// <inheritdoc/>
        void IDisposable.Dispose()
        {
            if (_context.Exceptions is not null)
            {
                if (_context.Exceptions.Count == 1)
                {
                    _context.Exceptions[0].Throw();
                }
                else
                {
                    throw new AggregateException(_context.Exceptions.Select(x => x.SourceException));
                }
            }
        }

        /// <summary>
        /// Invokes a C# callback.
        /// </summary>
        /// <param name="argument">The caller-provided argument.</param>
        /// <param name="data">A pointer to the <see cref="Context"/> to invoke.</param>
        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        private static void InvokeCallback(void* argument, void* data)
        {
            ref var context = ref Unsafe.AsRef<Context>(data);
            try
            {
                context.F((nuint)argument);
            }
            catch (Exception e)
            {
                context.Exceptions ??= new List<ExceptionDispatchInfo>();
                context.Exceptions.Add(ExceptionDispatchInfo.Capture(e));
            }
        }

        /// <summary>
        /// Data to pass across the FFI boundary during calls.
        /// Includes the user-provided function and every exception that it has thrown.
        /// </summary>
        private struct Context
        {
            /// <summary>
            /// The managed function.
            /// </summary>
            public Action<nuint> F;

            /// <summary>
            /// The exceptions that <see cref="F"/> has thrown, in the order they occurred, or
            /// <see langword="null"/> if it has not yet thrown.
            /// </summary>
            public List<ExceptionDispatchInfo>? Exceptions;
        }
    }

    /// <summary>
    /// Serializes an instance of this value.
    /// </summary>
    /// <param name="serializer">The serializer to use.</param>
    internal static void Serialize(BincodeSerializer serializer, EguiCallback obj)
    {
        serializer.increase_container_depth();
        serializer.serialize_u64((ulong)obj.func);
        serializer.serialize_u64((ulong)obj.data);
        serializer.decrease_container_depth();
    }

    /// <summary>
    /// Deserializes an instance of this value.
    /// </summary>
    /// <param name="deserializer">The deserializer to use.</param>
    /// <returns>The object that was deserialized.</returns>
    internal static EguiCallback Deserialize(BincodeDeserializer deserializer)
    {
        deserializer.increase_container_depth();
        EguiCallback obj = default;
        obj.func = (delegate* unmanaged[Cdecl]<void*, void*, void>)deserializer.deserialize_u64();
        obj.data = (void*)deserializer.deserialize_u64();
        deserializer.decrease_container_depth();
        return obj;
    }
}
