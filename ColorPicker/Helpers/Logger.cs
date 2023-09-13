using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Log(message + Environment.NewLine +
                ex.Message + Environment.NewLine +
                "Inner exception: " + Environment.NewLine +
                ex.InnerException?.Message + Environment.NewLine +
                "Stack trace: " + Environment.NewLine +
                ex.StackTrace,
                "ERROR");
        }

        public static void LogWarning(string message)
        {
            Log(message, "WARNING");
        }

        public static void LogInfo(string message)
        {
            Log(message, "INFO");
        }

        private static void Log(string message, string type)
        {
            var info = $"{GetCallerInfo()}\n\n{message}\nDo you want copy error message?";
            var title = $"{type}: {DateTime.Now.TimeOfDay}";
            var result = System.Windows.MessageBox.Show(info, title, (MessageBoxButton)MessageBoxButtons.YesNo, (MessageBoxImage)MessageBoxIcon.Error);

            if (result == MessageBoxResult.Yes)
            {
                System.Windows.Clipboard.SetText(message);
            }
        }


        private static string GetCallerInfo()
        {
            StackTrace stackTrace = new StackTrace();

            var methodName = stackTrace.GetFrame(3)?.GetMethod();
            var className = methodName?.DeclaringType.Name;
            return "[Method]: " + methodName.Name + " [Class]: " + className;
        }
    }
}
