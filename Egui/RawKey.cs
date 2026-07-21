namespace Egui;

public partial struct RawKey
{
    /// <summary>
    /// Like the real Rust <c>RawKey::new</c>, but takes an explicit <see cref="Egui.TypeId"/>
    /// instead of a Rust generic <c>T</c> (which cannot be supplied from C#). See
    /// <see cref="IdTypeMap.GetTempRaw"/>/<see cref="RawValue"/> for how to obtain a
    /// <see cref="Egui.TypeId"/>, or just pick one of your own consistently.
    /// </summary>
    public static RawKey New(TypeId typeId, Id id)
    {
        return EguiMarshal.Call<TypeId, Id, RawKey>(EguiFn.egui_util_id_type_map_RawKey_new, typeId, id);
    }
}
