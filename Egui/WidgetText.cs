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

    /// <summary>
    /// Layout with wrap mode based on the containing <see cref="Ui"/>.
    /// </summary>
    /// <param name="ui"></param>
    /// <param name="wrapMode">Override for <see cref="Ui.WrapMode"/></param>
    /// <param name="availableWidth"></param>
    /// <param name="fallbackFont"></param>
    public Egui.Galley IntoGalley(Ui ui, TextWrapMode? wrapMode, float availableWidth, FontSelection fallbackFont)
    {
        return new Egui.Galley(EguiMarshal.Call<WidgetText, nuint, TextWrapMode?, float, FontSelection, EguiHandle>(
            EguiFn.egui_widget_text_WidgetText_into_galley, this, ui.Ptr, wrapMode, availableWidth, fallbackFont));
    }

    /// <inheritdoc cref="IntoGalley"/>
    public Egui.Galley IntoGalleyImpl(Context ctx, Style style, Egui.Text.TextWrapping textWrapping, FontSelection fallbackFont, Align defaultValign)
    {
        return new Egui.Galley(EguiMarshal.Call<WidgetText, nuint, Style, Egui.Text.TextWrapping, FontSelection, Align, EguiHandle>(
            EguiFn.egui_widget_text_WidgetText_into_galley_impl, this, ctx.Ptr, style, textWrapping, fallbackFont, defaultValign));
    }
}