using System.Collections.Immutable;

namespace Egui.Widgets;

public partial record struct FrameDurations
{
    public IEnumerable<Duration> All => EguiMarshal.Call<FrameDurations, ImmutableArray<Duration>>(EguiFn.egui_widgets_image_FrameDurations_all, this);
}