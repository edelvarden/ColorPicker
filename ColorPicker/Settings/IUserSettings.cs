using System.Collections.Generic;
using System.Drawing;

namespace ColorPicker.Settings
{
    public enum ColorFormat { hex, rgb, hsl, hwb, hsv, lab, cmyk, xyz, vec4, rgb565, decimalBE, decimalLE };

    public interface IUserSettings
    {
        SettingItem<bool> RunOnStartup { get; }

        SettingItem<string> ActivationShortcut { get; }

        SettingItem<string> DefaultShortcut { get; }

        SettingItem<bool> ShowColorName { get; }

        SettingItem<ColorFormat> SelectedColorFormat { get; }
    }
}
