using System.Collections.Immutable;

namespace Egui;

public partial struct KeyboardShortcut
{
    public string Format(ModifierNames names, bool isMac)
    {
        return EguiMarshal.Call<KeyboardShortcut, bool, ImmutableArray<string>, bool, string>(EguiFn.egui_data_input_keyboard_shortcut_KeyboardShortcut_format,
            this, names.IsShort, [names.Alt, names.Ctrl, names.Shift, names.MacAlt, names.MacCmd, names.Concat], isMac);
    }
}