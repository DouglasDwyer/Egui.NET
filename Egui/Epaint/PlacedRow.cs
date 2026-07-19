namespace Egui.Epaint.Text;

public partial struct PlacedRow
{
    /// <summary>
    /// Includes the implicit newline after the row, if any.
    /// </summary>
    public readonly ulong CharCountIncludingNewline()
    {
        return EguiMarshal.Call<PlacedRow, ulong>(EguiFn.epaint_text_text_layout_types_PlacedRow_char_count_including_newline, this);
    }
}
