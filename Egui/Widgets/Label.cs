namespace Egui.Widgets;

public partial record struct Label
{
    /// <summary>
    /// Do layout and position the galley in the ui, without painting it or adding widget info.
    /// </summary>
    public (EPos2, Galley, Response) LayoutInUi(Ui ui)
    {
        var (pos, galleyHandle, response) = EguiMarshal.Call<Label, nuint, (EPos2, EguiHandle, Response)>(
            EguiFn.egui_widgets_label_Label_layout_in_ui, this, ui.Ptr);
        return (pos, new Galley(galleyHandle), response);
    }
}
