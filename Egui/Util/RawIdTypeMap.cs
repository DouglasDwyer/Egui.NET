using System.Collections.Immutable;

namespace Egui.Util;

/// <summary>
/// A live handle to the real Rust <c>IdTypeMap</c> backing a <see cref="Egui.Context"/>'s
/// widget state (as opposed to <see cref="IdTypeMap"/>, which is a separate, purely C#-side
/// store keyed by <see cref="System.Type"/>).<br/>
///
/// A stored value's Rust type cannot be recovered across the C# FFI boundary, so this only
/// exposes the "raw" accessors (<see cref="GetTempRaw"/>, <see cref="GetTempRawMut"/>,
/// <see cref="RemoveTempRaw"/>, <see cref="TempKeys"/>), which operate on a
/// <see cref="RawKey"/>/<see cref="RawValue"/> rather than a generic type parameter.
/// </summary>
public readonly ref struct RawIdTypeMap
{
    /// <summary>
    /// A pointer to the underlying map object.
    /// </summary>
    internal readonly nuint Ptr;

    /// <summary>
    /// Creates a new map object that references the given pointer.
    /// </summary>
    /// <param name="ptr">The native object pointer.</param>
    internal RawIdTypeMap(nuint ptr)
    {
        Ptr = ptr;
    }

    /// <summary>
    /// Gets a handle to a value for a given raw key.<br/>
    ///
    /// Serialized values are ignored.
    /// </summary>
    public readonly RawValue? GetTempRaw(RawKey raw)
    {
        AssertInitialized();
        return EguiMarshal.Call<nuint, RawKey, RawValue?>(EguiFn.egui_util_id_type_map_IdTypeMap_get_temp_raw, Ptr, raw);
    }

    /// <summary>
    /// Gets a handle to a mutable value for a given raw key.<br/>
    ///
    /// Serialized values are ignored.
    /// </summary>
    public readonly RawValue? GetTempRawMut(RawKey raw)
    {
        AssertInitialized();
        return EguiMarshal.Call<nuint, RawKey, RawValue?>(EguiFn.egui_util_id_type_map_IdTypeMap_get_temp_raw_mut, Ptr, raw);
    }

    /// <summary>
    /// Removes a temporary value given a raw key, returning a handle to the value that was
    /// removed (if any).<br/>
    ///
    /// Serialized values are ignored.
    /// </summary>
    public readonly RawValue? RemoveTempRaw(RawKey raw)
    {
        AssertInitialized();
        return EguiMarshal.Call<nuint, RawKey, RawValue?>(EguiFn.egui_util_id_type_map_IdTypeMap_remove_temp_raw, Ptr, raw);
    }

    /// <summary>
    /// Returns all <see cref="RawKey"/>s to values in this map.<br/>
    ///
    /// The returned keys can only be used with this map. Serializable values are ignored.
    /// </summary>
    public readonly ImmutableArray<RawKey> TempKeys()
    {
        AssertInitialized();
        return EguiMarshal.Call<nuint, ImmutableArray<RawKey>>(EguiFn.egui_util_id_type_map_IdTypeMap_temp_keys, Ptr);
    }

    /// <summary>
    /// Throws an exception if this is a null object.
    /// </summary>
    internal readonly void AssertInitialized()
    {
        if (Ptr == 0) { throw new NullReferenceException("RawIdTypeMap instance was uninitialized"); }
    }
}
