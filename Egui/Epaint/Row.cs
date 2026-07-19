namespace Egui.Epaint.Text;

public partial struct Row
{
    /// <summary>
    /// Excludes the implicit newline after the row, if any.
    /// </summary>
    public readonly ulong CharCountExcludingNewline()
    {
        return EguiMarshal.Call<Row, ulong>(EguiFn.epaint_text_text_layout_types_Row_char_count_excluding_newline, this);
    }

    /// <summary>
    /// Closest char at the desired x coordinate in row-relative coordinates.
    /// Returns something in the range <c>[0, CharCountExcludingNewline()]</c>.
    /// </summary>
    /// <param name="desiredX">The desired x coordinate.</param>
    public readonly ulong CharAt(float desiredX)
    {
        return EguiMarshal.Call<Row, float, ulong>(EguiFn.epaint_text_text_layout_types_Row_char_at, this, desiredX);
    }

    /// <summary>
    /// The x offset, in row-relative coordinates, of the given character column.
    /// </summary>
    /// <param name="column">The character column.</param>
    public readonly float XOffset(ulong column)
    {
        return EguiMarshal.Call<Row, ulong, float>(EguiFn.epaint_text_text_layout_types_Row_x_offset, this, column);
    }
}
