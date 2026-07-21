namespace Egui.EguiExtras;

/// <summary>
/// Specifies the properties of a column, like its width range.
/// </summary>
public struct Column
{
    private byte _kind;
    private float _width;
    private bool? _resizable;
    private bool? _clip;
    private float? _atLeast;
    private float? _atMost;
    private bool? _autoSizeThisFrame;

    private Column(byte kind, float width)
    {
        _kind = kind;
        _width = width;
    }

    /// <summary>
    /// Automatically sized based on content.
    /// </summary>
    public static Column Auto() => AutoWithInitialSuggestion(100.0f);

    /// <summary>
    /// Automatically sized. The given fallback is a loose suggestion for the initial width,
    /// used to wrap cell contents; in most cases it is ignored.
    /// </summary>
    public static Column AutoWithInitialSuggestion(float suggestedWidth) => new Column(0, suggestedWidth);

    /// <summary>
    /// With this initial width.
    /// </summary>
    public static Column Initial(float width) => new Column(1, width);

    /// <summary>
    /// Always this exact width, never shrink or grow.
    /// </summary>
    public static Column Exact(float width) => new Column(2, width);

    /// <summary>
    /// Take all the space remaining after the other columns have been sized.<br/>
    /// If you have multiple <c>Remainder</c> columns, they all share the remaining space equally.
    /// </summary>
    public static Column Remainder() => new Column(3, 0.0f);

    /// <summary>
    /// Can this column be resized by dragging the column separator?<br/>
    /// If you don't call this, <c>TableBuilder.Resizable</c> is used as a fallback.
    /// </summary>
    public readonly Column Resizable(bool resizable)
    {
        var result = this;
        result._resizable = resizable;
        return result;
    }

    /// <summary>
    /// If <c>true</c>: allow the column to shrink enough to clip the contents.<br/>
    /// If <c>false</c>: the column will always be wide enough to contain all its content.<br/>
    /// Default: <c>false</c>.
    /// </summary>
    public readonly Column Clip(bool clip)
    {
        var result = this;
        result._clip = clip;
        return result;
    }

    /// <summary>
    /// Won't shrink below this width (in points). Default: 0.0.
    /// </summary>
    public readonly Column AtLeast(float atLeast)
    {
        var result = this;
        result._atLeast = atLeast;
        return result;
    }

    /// <summary>
    /// Won't grow above this width (in points). Default: <see cref="float.PositiveInfinity"/>.
    /// </summary>
    public readonly Column AtMost(float atMost)
    {
        var result = this;
        result._atMost = atMost;
        return result;
    }

    /// <summary>
    /// If set, the column will be automatically sized based on the content this frame.<br/>
    /// Do not set this every frame, just on a specific action.
    /// </summary>
    public readonly Column AutoSizeThisFrame(bool autoSizeThisFrame)
    {
        var result = this;
        result._autoSizeThisFrame = autoSizeThisFrame;
        return result;
    }

    internal readonly void Serialize(Bincode.BincodeSerializer serializer)
    {
        serializer.increase_container_depth();
        serializer.serialize_u8(_kind);
        serializer.serialize_f32(_width);
        Egui.TraitHelpers.serialize_option_bool(_resizable, serializer);
        Egui.TraitHelpers.serialize_option_bool(_clip, serializer);
        Egui.TraitHelpers.serialize_option_f32(_atLeast, serializer);
        Egui.TraitHelpers.serialize_option_f32(_atMost, serializer);
        Egui.TraitHelpers.serialize_option_bool(_autoSizeThisFrame, serializer);
        serializer.decrease_container_depth();
    }
}
