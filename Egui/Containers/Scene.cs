namespace Egui.Containers;

public partial record struct Scene
{
    /// <summary>
    /// <paramref name="sceneRect"/> contains the view bounds of the inner <see cref="Ui"/>.<br/>
    /// <paramref name="sceneRect"/> will be mutated by any panning/zooming done by the user. If scene_rect is somehow invalid (e.g. <see cref="Rect.Zero"/>), then it will be reset to the inner rect of the inner ui.<br/>
    /// You need to store the <paramref name="sceneRect"/> in your state between frames.
    /// </summary>
    public readonly InnerResponse Show(Ui parentUi, ref Rect sceneRect, Action<Ui> addContents)
    {
        parentUi.AssertInitialized();
        var ctx = parentUi.Ctx;
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var (response, newRect) = EguiMarshal.Call<nuint, Scene, Rect, EguiCallback, (Response, Rect)>(EguiFn.egui_containers_scene_Scene_show, parentUi.Ptr, this, sceneRect, callback);
        sceneRect = newRect;
        return new InnerResponse
        {
            Response = response
        };
    }

    /// <inheritdoc cref="Show"/>
    public readonly InnerResponse<R> Show<R>(Ui parentUi, ref Rect sceneRect, Func<Ui, R> addContents)
    {
        parentUi.AssertInitialized();
        var ctx = parentUi.Ctx;
        R result = default!;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var (response, newRect) = EguiMarshal.Call<nuint, Scene, Rect, EguiCallback, (Response, Rect)>(EguiFn.egui_containers_scene_Scene_show, parentUi.Ptr, this, sceneRect, callback);
        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }
}