using ColorPicker.Helpers;
using ColorPicker.Properties;
using Microsoft.Xaml.Behaviors;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;

namespace ColorPicker.Behaviors
{
    public class NotifyIconBehavior : Behavior<Window>
    {
        private SettingsWindowHelper _settingsWindowHelper;
        private NotifyIcon _notifyIcon = null;

        protected override void OnAttached()
        {
            System.Windows.Application.Current.Exit += Current_Exit;
            base.OnAttached();

            _settingsWindowHelper = Bootstrapper.Container.GetExportedValue<SettingsWindowHelper>();
            SetupTrayIcon();
        }

        private void SetupTrayIcon()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = Resources.icon,
                Text = "Color picker",
                ContextMenu = new ContextMenu()
            };
            _notifyIcon.Visible = true;

            _notifyIcon.ContextMenu.MenuItems.Add(new MenuItem("Settings", OnSettingsClick) { ShowShortcut = false });
            _notifyIcon.ContextMenu.MenuItems.Add(new MenuItem("Close", onCloseClick) { ShowShortcut = false });

            _notifyIcon.MouseClick += (s, e) =>
            {
                // Prevent open program window when open context menu on right click
                if (e.Button == MouseButtons.Left)
                {
                    _settingsWindowHelper.ShowSettings();
                }
            };
        }
        private void Current_Exit(object sender, ExitEventArgs e)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
        }

        private void onCloseClick(object sender, EventArgs e)
        {
            // fix icon hiding that remains in the tray
            _notifyIcon.Visible = false;

            Process.GetCurrentProcess().Kill();
        }

        private void OnSettingsClick(object sender, EventArgs e)
        {
            _settingsWindowHelper.ShowSettings();
        }
    }
}
