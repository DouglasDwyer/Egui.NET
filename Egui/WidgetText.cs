namespace Egui;

public partial record struct WidgetText
{
    public string RawText => EguiMarshal.Call<WidgetText, string>(EguiFn.egui_widget_text_WidgetText_text, this);

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator WidgetText(Egui.RichText value) => new WidgetText.RichText { Value = value };
    
    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator WidgetText(Egui.Text.LayoutJob value) => new WidgetText.LayoutJob { Value = value };
    
    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator WidgetText(Egui.Galley value) => new WidgetText.Galley { Value = value };

    /// <summary>
    /// Converts to this type from the input type.
    /// </summary>
    public static implicit operator WidgetText(string value) => new WidgetText.RichText { Value = value };
}