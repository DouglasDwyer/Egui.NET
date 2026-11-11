using System.Collections.Immutable;

namespace Egui.Plot;

public partial record struct Legend
{
    /// <summary>
    /// Specifies hidden items in the legend configuration to override the
    /// existing ones. This allows the legend traces' visibility to be
    /// controlled from the application code.
    /// </summary>
    public readonly Legend HiddenItems(IEnumerable<ItemId> hiddenItems)
    {
        var result = this;
        result._hiddenItems = hiddenItems.Distinct().ToImmutableArray();
        return result;
    }
}
