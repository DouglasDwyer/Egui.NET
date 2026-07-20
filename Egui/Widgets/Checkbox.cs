namespace Egui.Widgets;

/// <summary>
/// Boolean on/off control with text label.<br/>
/// Usually you'd use <see cref="Ui.Checkbox"/> instead. 
/// </summary>
public ref struct Checkbox : IWidget
{
    private Atoms _atoms;
    private ref bool _checked;
    private bool _indeterminate;

    public Checkbox(ref bool isChecked, Atoms atoms)
    {
        _atoms = atoms;
        _checked = ref isChecked;
        _indeterminate = false;
    }

    /// <summary>
    /// Output the checkbox's <see cref="Egui.Atoms"/>.<br/>
    ///
    /// This includes any images you have on the checkbox.
    /// </summary>
    public readonly Atoms Atoms => _atoms;

    public static Checkbox WithoutText(ref bool isChecked)
    {
        return new Checkbox(ref isChecked, new Atoms());
    }

    /// <summary>
    /// Display an indeterminate state (neither checked nor unchecked)<br/>
    ///
    /// This only affects the checkbox's appearance. It will still toggle its boolean value when
    /// clicked.
    /// </summary>
    public readonly Checkbox Indeterminate(bool indeterminate)
    {
        var result = this;
        result._indeterminate = indeterminate;
        return result;
    }

    /// <inheritdoc/>
    Response IWidget.Ui(Ui ui)
    {
        ui.AssertInitialized();
        var (response, setChecked) = EguiMarshal.Call<nuint, Atoms, bool, bool, (Response, bool)>(EguiFn.egui_widgets_checkbox_Checkbox_ui, ui.Ptr, _atoms, _checked, _indeterminate);
        _checked = setChecked;
        return response;
    }
}