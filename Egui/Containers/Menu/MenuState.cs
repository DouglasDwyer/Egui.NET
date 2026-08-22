using Microsoft.VisualBasic;

namespace Egui.Containers.Menu;

public partial record struct MenuState
{
    /// <inheritdoc cref="MutateMenuStateFromUi{R}"/>
    public delegate void MutateMenuStateFromUi(ref MenuState state, UiStack stack);

    /// <summary>
    /// Callback for the <see cref="FromUi"/> function. 
    /// </summary>
    /// <typeparam name="R">The type to return</typeparam>
    /// <param name="state">The state to inspect.</param>
    /// <param name="stack">The current UI stack.</param>
    /// <returns>A user-provided value</returns>
    public delegate R MutateMenuStateFromUi<R>(ref MenuState state, UiStack stack);

    /// <summary>
    /// Find the root of the menu and get the state
    /// </summary>
    public static void FromUi(Ui ui, MutateMenuStateFromUi f)
    {
        var stack = MenuHelpers.FindMenuRoot(ui);
        FromId(ui.Ctx, stack.Id, (ref MenuState state) => f(ref state, stack));
    }

    /// <inheritdoc cref="FromUi"/>
    public static R FromUi<R>(Ui ui, MutateMenuStateFromUi<R> f)
    {
        var stack = MenuHelpers.FindMenuRoot(ui);
        return FromId(ui.Ctx, stack.Id, (ref MenuState state) => f(ref state, stack));
    }

    public static void FromId(Context ctx, Id id, MutateDelegate<MenuState> f)
    {
        FromId(ctx, id, (ref MenuState state) =>
        {
            f(ref state);
            return false;
        });
    }

    public static R FromId<R>(Context ctx, Id id, MutateDelegate<MenuState, R> f)
    {
        throw new NotImplementedException();
    }
}