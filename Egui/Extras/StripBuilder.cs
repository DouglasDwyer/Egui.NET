using System.ComponentModel;

namespace Egui.EguiExtras;

/// <summary>
/// Builder for creating a new <see cref="Strip"/>. This can be used to do dynamic layouts.<br/>
///
/// In contrast to normal <see cref="Ui"/> behavior, strip cells do <i>not</i> grow with their
/// children.<br/>
///
/// First use <see cref="Size"/>/<see cref="Sizes"/> to allocate space for the rows or columns
/// that will follow. Then build the strip with <see cref="Horizontal"/>/<see cref="Vertical"/>,
/// and add cells to it using <see cref="Strip.Cell"/>. The number of cells must match the
/// number of pre-allocated sizes.
/// </summary>
public ref struct StripBuilder
{
    private Ui _ui;
    private bool _clip;
    private Layout? _cellLayout;
    private Egui.Sense _sense;
    private List<Size> _sizes;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("'StripBuilder' does not contain a constructor that takes 0 arguments", error: true)]
    public StripBuilder() { throw new InvalidOperationException(); }

    public StripBuilder(Ui ui)
    {
        _ui = ui;
        _clip = false;
        _sense = Egui.Sense.Hover;
        _sizes = new List<Size>();
    }

    /// <summary>
    /// Should we clip the contents of each cell? Default: <c>false</c>.
    /// </summary>
    public readonly StripBuilder Clip(bool clip)
    {
        var result = this;
        result._clip = clip;
        return result;
    }

    /// <summary>
    /// What layout should we use for the individual cells?
    /// </summary>
    public readonly StripBuilder CellLayout(Layout cellLayout)
    {
        var result = this;
        result._cellLayout = cellLayout;
        return result;
    }

    /// <summary>
    /// What should strip cells sense for? Default: <see cref="Egui.Sense.Hover"/>.
    /// </summary>
    public readonly StripBuilder Sense(Egui.Sense sense)
    {
        var result = this;
        result._sense = sense;
        return result;
    }

    /// <summary>
    /// Allocate space for one column/row.
    /// </summary>
    public readonly StripBuilder Size(Size size)
    {
        var result = this;
        result._sizes = new List<Size>(_sizes) { size };
        return result;
    }

    /// <summary>
    /// Allocate space for several columns/rows at once.
    /// </summary>
    public readonly StripBuilder Sizes(Size size, int count)
    {
        var result = this;
        var newSizes = new List<Size>(_sizes);
        for (int i = 0; i < count; i++)
        {
            newSizes.Add(size);
        }
        result._sizes = newSizes;
        return result;
    }

    /// <summary>
    /// Build horizontal strip: cells are positioned from left to right.<br/>
    ///
    /// Takes the available horizontal width, so there can't be anything right of the strip or
    /// the container will grow slowly.
    /// </summary>
    public readonly Response Horizontal(Action<Strip> addStripContents) => Show(EguiFn.egui_extras_strip_StripBuilder_horizontal, addStripContents);

    /// <summary>
    /// Build vertical strip: cells are positioned from top to bottom.<br/>
    ///
    /// Takes the full available vertical height, so there can't be anything below the strip or
    /// the container will grow slowly.
    /// </summary>
    public readonly Response Vertical(Action<Strip> addStripContents) => Show(EguiFn.egui_extras_strip_StripBuilder_vertical, addStripContents);

    private readonly Response Show(EguiFn func, Action<Strip> addStripContents)
    {
        _ui.AssertInitialized();
        var ctx = _ui.Ctx;
        var options = ToOptions();

        using var callback = new EguiCallback(stripPtr => addStripContents(new Strip(ctx, stripPtr)));
        return EguiMarshal.Call<nuint, StripOptions, EguiCallback, Response>(func, _ui.Ptr, options, callback);
    }

    private readonly StripOptions ToOptions() => new StripOptions
    {
        Clip = _clip,
        CellLayout = _cellLayout,
        Sense = _sense,
        Sizes = _sizes
    };

    /// <summary>
    /// The wire representation of a <see cref="StripBuilder"/>'s options, sent alongside the
    /// <see cref="Ui"/> pointer and callback when the strip is shown.
    /// </summary>
    private struct StripOptions
    {
        public bool Clip;
        public Layout? CellLayout;
        public Egui.Sense Sense;
        public List<Size> Sizes;

        internal static void Serialize(Bincode.BincodeSerializer serializer, StripOptions value) => value.Serialize(serializer);

        internal readonly void Serialize(Bincode.BincodeSerializer serializer)
        {
            serializer.increase_container_depth();

            serializer.serialize_bool(Clip);

            if (CellLayout is Layout cellLayout)
            {
                serializer.serialize_option_tag(true);
                cellLayout.Serialize(serializer);
            }
            else
            {
                serializer.serialize_option_tag(false);
            }

            Sense.Serialize(serializer);

            serializer.serialize_len(Sizes.Count);
            foreach (var size in Sizes)
            {
                size.Serialize(serializer);
            }

            serializer.decrease_container_depth();
        }

        internal static StripOptions Deserialize(Bincode.BincodeDeserializer deserializer) => throw new NotImplementedException();
    }
}
