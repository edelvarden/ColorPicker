using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace ColorPicker.Helpers
{
    public static class Logger
    {
        public static string ApplicationLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ColorPicker");

        public static void LogError(string message)
        {
            Log(message, "ERROR");
        }

        public static void LogError(string message, Exception ex)
        {
            Log($"{message}\n{ex.Message}\nInner exception:\n{ex.InnerException?.Message}\nStack trace:\n{ex.StackTrace}", "ERROR");
        }

        public static void LogWarning(string message)
        {
            Log(message, "WARNING");
        }

        public static void LogInfo(string message)
        {
            Log(message, "INFO");
        }

        private static void Log(string message, string type, [CallerMemberName] string caller = "")
        {
            var info = $"{caller}\n\n{message}\nDo you want to copy the error message?";
            var title = $"{type}: {DateTime.Now.TimeOfDay}";
            var result = System.Windows.MessageBox.Show(info, title, (MessageBoxButton)MessageBoxButtons.YesNo, (MessageBoxImage)MessageBoxIcon.Error);

            if (result == MessageBoxResult.Yes)
            {
                System.Windows.Clipboard.SetText(message);
            }
        }
    }
}
