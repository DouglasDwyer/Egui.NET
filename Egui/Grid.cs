namespace Egui;

#pragma warning disable CS0282
public partial record struct Grid
#pragma warning restore
{
    /// <summary>
    /// Whether to show the grid as striped.
    /// </summary>
    private bool? _striped;

    /// <summary>
    /// If <c>True</c>, add a subtle background color to every other row.<br/>
    /// 
    /// This can make a table easier to read.
    /// Default is whatever is in <c>Striped</c>.
    /// </summary>
    public Grid Striped(bool striped)
    {
        var result = this;
        result._striped = true;
        return result;
    }

    public readonly InnerResponse Show(Ui ui, Action<Ui> addContents)
    {
        Style style = default;

        if (_striped.HasValue)
        {
            style = ui.Style;
            var newStyle = style;
            newStyle.Visuals.Striped = _striped.Value;
            ui.SetStyle(newStyle);
        }

        var ctx = ui.Ctx;
        using var callback = new EguiCallback(ui => addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, Grid, EguiCallback, Response>(EguiFn.egui_grid_Grid_show, ui.Ptr, this, callback);

        if (_striped.HasValue)
        {
            ui.SetStyle(style);
        }

        return new InnerResponse
        {
            Response = response
        };
    }

    public readonly InnerResponse<R> Show<R>(Ui ui, Func<Ui, R> addContents)
    {
        Style style = default;

        if (_striped.HasValue)
        {
            style = ui.Style;
            var newStyle = style;
            newStyle.Visuals.Striped = _striped.Value;
            ui.SetStyle(newStyle);
        }

        R result = default!;
        var ctx = ui.Ctx;
        using var callback = new EguiCallback(ui => result = addContents(new Ui(ctx, ui)));
        var response = EguiMarshal.Call<nuint, Grid, EguiCallback, Response>(EguiFn.egui_grid_Grid_show, ui.Ptr, this, callback);

        if (_striped.HasValue)
        {
            ui.SetStyle(style);
        }

        return new InnerResponse<R>
        {
            Inner = result,
            Response = response
        };
    }
}