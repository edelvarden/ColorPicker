using System.Collections.Generic;
using System.Drawing;

namespace ColorPicker.Settings
{
    public enum ColorFormat { hex, rgb, hsl, hsv, vec4, rgb565, decimalBE, decimalLE, cmyk, lab };

    public interface IUserSettings
    {
        SettingItem<bool> RunOnStartup { get; }

        SettingItem<string> ActivationShortcut { get; }

        SettingItem<string> DefaultShortcut { get; }

        SettingItem<bool> ShowColorName { get; }

        SettingItem<ColorFormat> SelectedColorFormat { get; }
    }
}
