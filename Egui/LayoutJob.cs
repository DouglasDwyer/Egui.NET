namespace Egui.Text;

public partial struct LayoutJob
{
    /// <summary>
    /// Gets the <see cref="TextFormat"/> of the section covering the given byte index.<br/>
    ///
    /// Panics if the job has no sections.
    /// </summary>
    /// <param name="byteIdx">The byte index into <see cref="Text"/>.</param>
    public readonly TextFormat FormatAtByte(ulong byteIdx)
    {
        return EguiMarshal.Call<LayoutJob, ulong, TextFormat>(EguiFn.epaint_text_text_layout_types_LayoutJob_format_at_byte, this, byteIdx);
    }
}
