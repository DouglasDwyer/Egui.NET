using System.Collections.Immutable;

namespace Egui;

/// <summary>
/// Text that has been laid out, ready for painting.<br/>
///
/// You can create a <see cref="Galley"/> using <see cref="Egui.Epaint.FontsView.LayoutJob"/>.<br/>
///
/// This is a reference-counted handle, so cloning and repeated layout of identical text is cheap.
/// </summary>
public sealed partial class Galley : EguiObject
{
    /// <summary>
    /// Creates a galley handle for the given underlying object.
    /// </summary>
    internal Galley(EguiHandle handle) : base(handle) { }

    /// <summary>
    /// The job that this galley is the result of.
    /// Contains the original string and style sections.
    /// </summary>
    public Egui.Text.LayoutJob Job => EguiMarshal.Call<nuint, Egui.Text.LayoutJob>(EguiFn.epaint_text_text_layout_types_Galley_job, Ptr);

    /// <summary>
    /// Rows of text, from top to bottom, and their offsets.
    /// </summary>
    public ImmutableArray<Egui.Epaint.Text.PlacedRow> Rows => EguiMarshal.Call<nuint, ImmutableArray<Egui.Epaint.Text.PlacedRow>>(EguiFn.epaint_text_text_layout_types_Galley_rows, Ptr);

    /// <summary>
    /// Set to true the text was truncated due to <c>TextWrapping.MaxRows</c>.
    /// </summary>
    public bool Elided => EguiMarshal.Call<nuint, bool>(EguiFn.epaint_text_text_layout_types_Galley_elided, Ptr);

    /// <summary>
    /// Bounding rect.
    /// </summary>
    public Rect Rect => EguiMarshal.Call<nuint, Rect>(EguiFn.epaint_text_text_layout_types_Galley_rect, Ptr);

    /// <summary>
    /// Tight bounding box around all the meshes in all the rows.
    /// Can be used for culling.
    /// </summary>
    public Rect MeshBounds => EguiMarshal.Call<nuint, Rect>(EguiFn.epaint_text_text_layout_types_Galley_mesh_bounds, Ptr);

    /// <summary>
    /// Total number of vertices in all the row meshes.
    /// </summary>
    public nuint NumVertices => EguiMarshal.Call<nuint, nuint>(EguiFn.epaint_text_text_layout_types_Galley_num_vertices, Ptr);

    /// <summary>
    /// Total number of indices in all the row meshes.
    /// </summary>
    public nuint NumIndices => EguiMarshal.Call<nuint, nuint>(EguiFn.epaint_text_text_layout_types_Galley_num_indices, Ptr);

    /// <summary>
    /// The number of physical pixels for each logical point, at the time of layout.
    /// </summary>
    public float PixelsPerPoint => EguiMarshal.Call<nuint, float>(EguiFn.epaint_text_text_layout_types_Galley_pixels_per_point, Ptr);

    /// <summary>
    /// Append each galley under the previous one.
    /// </summary>
    public static Galley Concat(Egui.Text.LayoutJob job, IEnumerable<Galley> galleys, float pixelsPerPoint)
    {
        return new Galley(EguiMarshal.Call<Egui.Text.LayoutJob, ImmutableArray<nuint>, float, EguiHandle>(
            EguiFn.epaint_text_text_layout_types_Galley_concat, job, galleys.Select(x => x.Ptr).ToImmutableArray(), pixelsPerPoint));
    }

    /// <summary>
    /// Serializes an instance of this value.
    /// </summary>
    /// <param name="serializer">The serializer to use.</param>
    /// <param name="value">The value to serialize.</param>
    internal static void Serialize(Bincode.BincodeSerializer serializer, Galley value) => value.Serialize(serializer);

    /// <inheritdoc cref="Serialize(Bincode.BincodeSerializer, Galley)"/>
    internal void Serialize(Bincode.BincodeSerializer serializer) => EguiHandle.Serialize(serializer, Handle);

    /// <summary>
    /// Deserializes an instance of this value.
    /// </summary>
    /// <param name="deserializer">The deserializer to use.</param>
    /// <returns>The object that was deserialized.</returns>
    internal static Galley Deserialize(Bincode.BincodeDeserializer deserializer) => new Galley(EguiHandle.Deserialize(deserializer));
}
