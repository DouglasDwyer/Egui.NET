using System.Collections.Immutable;

namespace Egui;

/// <summary>
/// Names of different modifier keys.
/// Used to name modifiers.
/// </summary>
public struct ModifierNames
{
    /// <summary>
    /// ⌥ ⌃ ⇧ ⌘ - NOTE: not supported by the default egui font.
    /// </summary>
    public static readonly ModifierNames Symbols = new ModifierNames
    {
        IsShort = true,
        Alt = "⌥",
        Ctrl = "⌃",
        Shift = "⇧",
        MacCmd = "⌘",
        MacAlt = "⌥",
        Concat = ""
    };

    /// <summary>
    /// Alt, Ctrl, Shift, Cmd
    /// </summary>
    public static readonly ModifierNames Names = new ModifierNames
    {
        IsShort = true,
        Alt = "Alt",
        Ctrl = "Ctrl",
        Shift = "Shift",
        MacCmd = "Cmd",
        MacAlt = "Option",
        Concat = "+"
    };

    public required bool IsShort;
    public required string Alt;
    public required string Ctrl;
    public required string Shift;
    public required string MacAlt;
    public required string MacCmd;
    /// <summary>
    /// What goes between the names
    /// </summary>
    public required string Concat;

    public readonly string Format(Modifiers modifiers, bool isMac)
    {
        return EguiMarshal.Call<bool, ImmutableArray<string>, Modifiers, bool, string>(EguiFn.egui_data_input_modifier_names_ModifierNames_format,
            IsShort, [Alt, Ctrl, Shift, MacAlt, MacCmd, Concat], modifiers, isMac);
    }
}