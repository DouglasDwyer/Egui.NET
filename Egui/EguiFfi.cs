namespace Egui;

/// <summary>
/// Facilitates communication between <c>egui</c> and natively-loaded
/// Rust libraries.
/// </summary>
public sealed class EguiFfi : EguiObject
{
    /// <summary>
    /// Gets a pointer to the underlying object. This pointer can be used
    /// from Rust to access input/output data.<br/>
    /// Access to the pointer is not externally synchronized - it should only
    /// be used from one thread at a time.
    /// </summary>
    public nuint Pointer => Ptr;

    /// <summary>
    /// Creates a new FFI object.
    /// </summary>
    public EguiFfi() : base(EguiMarshal.Call<EguiHandle>(EguiFn.egui_EguiFfi_new)) { }
}