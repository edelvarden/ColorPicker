using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Documents;
using static ColorPicker.Helpers.NativeMethodsHelper;

namespace ColorPicker.Helpers
{
    public enum WindowType
    {
        ColorPicker,
        ZoomWindow
    }

    [Export(typeof(AppStateHandler))]
    public class AppStateHandler
    {
        private readonly List<WindowType> _currentlyShownApps = new List<WindowType>();
        private bool _colorPickerShown;
        private object _colorPickerVisibilityLock = new object();

        public event EventHandler EnterPressed;
        public event EventHandler<WindowType> AppShown;
        public event EventHandler<WindowType> AppHidden;
        public event EventHandler AppClosed;

        public void ShowColorPicker()
        {
            lock (_colorPickerVisibilityLock)
            {
                AddShownApp(WindowType.ColorPicker);
                AppShown?.Invoke(this, WindowType.ColorPicker);
                Application.Current.MainWindow.Opacity = 1;
                Application.Current.MainWindow.Visibility = Visibility.Visible;
                _colorPickerShown = true;
            }
        }

        public void HideColorPicker()
        {
            lock (_colorPickerVisibilityLock)
            {
                Application.Current.MainWindow.Opacity = 0;
                Application.Current.MainWindow.Visibility = Visibility.Collapsed;
                RemoveShownApp(WindowType.ColorPicker);
                AppHidden?.Invoke(this, WindowType.ColorPicker);
                _colorPickerShown = false;
            }
        }

        public bool IsColorPickerVisible()
        {
            return _colorPickerShown;
        }

        public void SetTopMost()
        {
            Application.Current.MainWindow.Topmost = false;
            Application.Current.MainWindow.Topmost = true;
        }

        private void AddShownApp(WindowType type)
        {
            if (!_currentlyShownApps.Contains(type))
            {
                _currentlyShownApps.Add(type);
            }
        }

        private void RemoveShownApp(WindowType type)
        {
            if (_currentlyShownApps.Contains(type))
            {
                _currentlyShownApps.Remove(type);
            }
        }

        internal void MoveCursor(int xOffset, int yOffset)
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            lpPoint.X += xOffset;
            lpPoint.Y += yOffset;
            SetCursorPos(lpPoint.X, lpPoint.Y);
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            AppClosed?.Invoke(this, EventArgs.Empty);
        }

        public bool HandleEnterPressed()
        {
            if (!IsColorPickerVisible())
            {
                return false;
            }

            EnterPressed?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}
