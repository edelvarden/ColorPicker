using ColorPicker.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;

namespace ColorPicker.Settings
{
    [Export(typeof(IUserSettings))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class UserSettings : IUserSettings
    {

        [ImportingConstructor]
        public UserSettings()
        {
            var settings = Properties.Settings.Default;

            RunOnStartup = new SettingItem<bool>(settings.RunOnStartup, (currentValue) => { settings.RunOnStartup = currentValue; SaveSettings(); });
            ActivationShortcut = new SettingItem<string>(settings.ActivationShortcut, (currentValue) => { settings.ActivationShortcut = currentValue; SaveSettings(); });
            ShowColorName = new SettingItem<bool>(settings.ShowColorName, (currentValue) => { settings.ShowColorName = currentValue; SaveSettings(); });
            SelectedColorFormat = new SettingItem<ColorFormat>((ColorFormat)Enum.Parse(typeof(ColorFormat), settings.SelectedColorFormat, true), (currentValue) => { settings.SelectedColorFormat = currentValue.ToString(); SaveSettings(); });
        }

        public SettingItem<bool> RunOnStartup { get; }

        public SettingItem<string> ActivationShortcut { get; }

        public SettingItem<bool> ChangeCursor { get; }

        public SettingItem<bool> ShowColorName { get; }

        public SettingItem<ColorFormat> SelectedColorFormat { get; }

        private void SaveSettings()
        {
            Properties.Settings.Default.Save();
        }
    }
}
