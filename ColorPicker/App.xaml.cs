using ColorPicker.Helpers;
using ColorPicker.Mouse;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;

namespace ColorPicker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex _instanceMutex = null;

        [STAThread]
        public static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            try
            {
                var application = new App();
                application.InitializeComponent();
                application.Run();
            }
            catch (Exception ex)
            {
                Logger.LogError("Unhandled exception", ex);
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            LoadSettings();
            // allow only one instance of color picker
            bool createdNew;
            _instanceMutex = new Mutex(true, @"Global\ControlPanel", out createdNew);
            if (!createdNew)
            {
                _instanceMutex = null;
                Application.Current.Shutdown();
                return;
            }

            base.OnStartup(e);
            LoadTheme();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.LogError("Unhandled exception", (e.ExceptionObject is Exception) ? (e.ExceptionObject as Exception) : new Exception());
        }

        private void LoadTheme()
        {
            bool isDarkTheme = IsSystemInDarkMode();

            var themeDictionary = new ResourceDictionary();
            if (isDarkTheme)
            {
                themeDictionary.Source = new Uri("pack://application:,,,/Resources/DarkTheme.xaml");
            }
            else
            {
                themeDictionary.Source = new Uri("pack://application:,,,/Resources/LightTheme.xaml");
            }

            Resources.MergedDictionaries.Add(themeDictionary);
        }

        private bool IsSystemInDarkMode()
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

        private void LoadSettings()
        {
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string settingsFilePath = exePath + ".config";

            if (!File.Exists(settingsFilePath))
            {
                using (StreamWriter sw = new StreamWriter(settingsFilePath))
                {
                    // Write the configuration data to the file.
                    sw.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n");
                    sw.Write("<configuration>\n");
                    sw.Write("    <configSections>\n");
                    sw.Write("        <sectionGroup name=\"userSettings\" type=\"System.Configuration.UserSettingsGroup, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\">\n");
                    sw.Write("            <section name=\"ColorPicker.Properties.Settings\" type=\"System.Configuration.ClientSettingsSection, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\" allowExeDefinition=\"MachineToLocalUser\" requirePermission=\"false\" />\n");
                    sw.Write("        </sectionGroup>\n");
                    sw.Write("    </configSections>\n");
                    sw.Write("    <startup>\n");
                    sw.Write("        <supportedRuntime version=\"v4.0\" sku=\".NETFramework,Version=v4.8\" />\n");
                    sw.Write("    </startup>\n");
                    sw.Write("    <userSettings>\n");
                    sw.Write("        <ColorPicker.Properties.Settings>\n");
                    sw.Write("            <setting name=\"RunOnStartup\" serializeAs=\"String\">\n");
                    sw.Write("                <value>False</value>\n");
                    sw.Write("            </setting>\n");
                    sw.Write("            <setting name=\"UpdateSettings\" serializeAs=\"String\">\n");
                    sw.Write("                <value>True</value>\n");
                    sw.Write("            </setting>\n");
                    sw.Write("            <setting name=\"ActivationShortcut\" serializeAs=\"String\">\n");
                    sw.Write("                <value>LWin + C</value>\n");
                    sw.Write("            </setting>\n");
                    sw.Write("            <setting name=\"SelectedColorFormat\" serializeAs=\"String\">\n");
                    sw.Write("                <value>HEX</value>\n");
                    sw.Write("            </setting>\n");
                    sw.Write("            <setting name=\"ShowColorName\" serializeAs=\"String\">\n");
                    sw.Write("                <value>False</value>\n");
                    sw.Write("            </setting>\n");
                    sw.Write("        </ColorPicker.Properties.Settings>\n");
                    sw.Write("    </userSettings>\n");
                    sw.Write("</configuration>\n");
                }

                // Restart program to read config file again
                Process.Start(exePath);
                Process.GetCurrentProcess().Kill();
                return;
            }
        }

    }
}
