using Microsoft.Win32;
using System;
using System.Reflection;

namespace ColorPicker.Helpers
{
    static class RegistryHelper
    {
        private const string AppName = "ColorPicker";

        public static bool SetRunOnStartup(bool enabled)
        {
            try
            {
                using (RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    if (enabled)
                    {
                        rk.SetValue(AppName, Assembly.GetExecutingAssembly().Location);
                    }
                    else
                    {
                        rk.DeleteValue(AppName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to set run on startup", ex);
                return false;
            }
            return true;
        }

        public static bool IsSystemInDarkMode()
        {
            const string registryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            const string registryValueName = "AppsUseLightTheme";

            bool isDarkMode = false;

            // Open the registry key
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKeyPath))
            {
                if (key != null)
                {
                    // Read the registry value
                    object value = key.GetValue(registryValueName);

                    if (value != null && int.TryParse(value.ToString(), out int intValue))
                    {
                        // Check if AppsUseLightTheme is set to 0 (dark mode)
                        isDarkMode = (intValue == 0);
                    }
                }
            }

            return isDarkMode;
        }
    }
}
