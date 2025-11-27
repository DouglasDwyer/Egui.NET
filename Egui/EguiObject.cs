using System.ComponentModel;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Egui;

/// <exclude/>
[EditorBrowsable(EditorBrowsableState.Never)]
public class EguiObject
{
    /// <summary>
    /// A pointer to the underlying object pointer.
    /// </summary>
    internal nuint Ptr => _handle.ptr;

    /// <summary>
    /// The underlying handle that represents the heap-allocated Rust object.
    /// </summary>
    private readonly EguiHandle _handle;

    /// <summary>
    /// Creates a new <c>egui</c> object.
    /// </summary>
    /// <param name="handle">The handle representing the object.</param>
    internal EguiObject(EguiHandle handle)
    {
        _handle = handle;
        ObjectFreeEvent.Register(this, () => EguiBindings.egui_drop(handle));
    }

    /// <summary>
    /// Runs a closure only after an object is well and truly dead.
    /// </summary>
    private sealed class ObjectFreeEvent {
        /// <summary>
        /// The action to run.
        /// </summary>
        private Action? _action;

        /// <summary>
        /// Ensures that this event's finalizer does not run spuriously.
        /// </summary>
        private readonly DependentHandle _keepAliveHandle;

        /// <summary>
        /// A long weak reference to the target object.
        /// </summary>
        public readonly GCHandle _target;
        
        /// <inheritdoc cref="Register"/> 
        private ObjectFreeEvent(object target, Action action)
        {
            _action = action;
            _keepAliveHandle = new DependentHandle(target, this);
            _target = GCHandle.Alloc(target, GCHandleType.WeakTrackResurrection);
        }

        /// <summary>
        /// Registers a callback to invoke when <paramref name="target"/> is collected.
        /// </summary>
        /// <param name="target">The object to track.</param>
        /// <param name="action">An action to run after it is freed.</param>
        public static void Register(object target, Action action)
        {
            new ObjectFreeEvent(target, action);
        }
        
        /// <summary>
        /// Invokes the <see cref="_action"/> if the target object has been collected. 
        /// </summary>
        ~ObjectFreeEvent()
        {
            if (_target.Target is null)
            {
                if (Interlocked.Exchange(ref _action, null) is Action action)
                {
                    _keepAliveHandle.Dispose();
                    _target.Free();
                    action();
                }
            }
            else
            {
                GC.ReRegisterForFinalize(this);
            }
        }
    }
}