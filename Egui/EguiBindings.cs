namespace Egui
{
    /// <summary>
    /// Provides access to the underlying C functions used to communicate with <c>egui</c>.
    /// </summary>
    internal static partial class EguiBindings
    {
        static EguiBindings()
        {
            unsafe
            {
                // Safety: initializing the library has no preconditions.
                egui_init();
            }
        }
    }
}
