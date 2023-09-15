using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Interop;
using ColorPicker.Settings;
using ColorPicker.ViewModelContracts;

namespace ColorPicker.Helpers
{
    [Export(typeof(AppStateHandler))]
    public class AppStateHandler
    {
        private readonly IUserSettings _userSettings;
        private bool _colorPickerShown;
        private object _colorPickerVisibilityLock = new object();

        private HwndSource _hwndSource;
        private const int _globalHotKeyId = 0x0001;

        // Blocks using the escape key to close the color picker editor when the adjust color flyout is open.
        public static bool BlockEscapeKeyClosingColorPickerEditor { get; set; }

        [ImportingConstructor]
        public AppStateHandler(IUserSettings userSettings)
        {
            Application.Current.MainWindow.Closed += MainWindow_Closed;
            _userSettings = userSettings;
        }

        public event EventHandler AppShown;

        public event EventHandler AppHidden;

        public event EventHandler AppClosed;

        public void StartUserSession()
        {
            EndUserSession(); // Ends current user session if there's an active one.
            lock (_colorPickerVisibilityLock)
            {

                ShowColorPicker();

                // Handle the escape key to close Color Picker locally when being spawn from PowerToys, since Keyboard Hooks from the KeyboardMonitor are heavy.
                SetupEscapeGlobalKeyShortcut();
            }
        }

        public bool EndUserSession()
        {
            lock (_colorPickerVisibilityLock)
            {
                if (_colorPickerShown)
                {
                    HideColorPicker();

                    // Handle the escape key to close Color Picker locally when being spawn from PowerToys, since Keyboard Hooks from the KeyboardMonitor are heavy.
                    ClearEscapeGlobalKeyShortcut();

                    return true;
                }

                return false;
            }
        }

        public void OnColorPickerMouseDown()
        {
            lock (_colorPickerVisibilityLock)
            {
                HideColorPicker();
            }
            EndUserSession();
        }

        public static void SetTopMost()
        {
            Application.Current.MainWindow.Topmost = false;
            Application.Current.MainWindow.Topmost = true;
        }

        public void ShowColorPicker()
        {
            if (!_colorPickerShown)
            {
                AppShown?.Invoke(this, EventArgs.Empty);
                Application.Current.MainWindow.Opacity = 0;
                Application.Current.MainWindow.Visibility = Visibility.Visible;
                _colorPickerShown = true;
            }
        }

        public void HideColorPicker()
        {
            if (_colorPickerShown)
            {
                Application.Current.MainWindow.Opacity = 0;
                Application.Current.MainWindow.Visibility = Visibility.Collapsed;
                AppHidden?.Invoke(this, EventArgs.Empty);
                _colorPickerShown = false;
            }
        }

        public bool IsColorPickerVisible()
        {
            return _colorPickerShown;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            AppClosed?.Invoke(this, EventArgs.Empty);
        }

        private void ColorEditorViewModel_OpenColorPickerRequested(object sender, EventArgs e)
        {
            lock (_colorPickerVisibilityLock)
            {
                ShowColorPicker();
            }
        }

        internal void RegisterWindowHandle(System.Windows.Interop.HwndSource hwndSource)
        {
            _hwndSource = hwndSource;
        }

        public IntPtr ProcessWindowMessages(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            switch (msg)
            {
                case NativeMethods.WM_HOTKEY:
                    if (!BlockEscapeKeyClosingColorPickerEditor)
                    {
                        handled = EndUserSession();
                    }
                    else
                    {
                        // If escape key is blocked it means a submenu is open.
                        // Send the escape key to the Window to close that submenu.
                        // Description for LPARAM in https://learn.microsoft.com/windows/win32/inputdev/wm-keyup#parameters
                        // It's basically some modifiers + scancode for escape (1) + number of repetitions (1)
                        handled = true;
                        handled &= NativeMethods.PostMessage(_hwndSource.Handle, NativeMethods.WM_KEYDOWN, (IntPtr)NativeMethods.VK_ESCAPE, (IntPtr)0x00010001);

                        // Need to make the value unchecked as a workaround for changes introduced in .NET 7
                        // https://github.com/dotnet/roslyn/blob/main/docs/compilers/CSharp/Compiler%20Breaking%20Changes%20-%20DotNet%207.md#checked-operators-on-systemintptr-and-systemuintptr
                        handled &= NativeMethods.PostMessage(_hwndSource.Handle, NativeMethods.WM_KEYUP, (IntPtr)NativeMethods.VK_ESCAPE, unchecked((IntPtr)0xC0010001));
                    }

                    break;
            }

            return IntPtr.Zero;
        }

        public void SetupEscapeGlobalKeyShortcut()
        {
            if (_hwndSource == null)
            {
                return;
            }

            _hwndSource.AddHook(ProcessWindowMessages);
            if (!NativeMethods.RegisterHotKey(_hwndSource.Handle, _globalHotKeyId, NativeMethods.MOD_NOREPEAT, NativeMethods.VK_ESCAPE))
            {
                Logger.LogWarning("Couldn't register the hotkey for Esc.");
            }
        }

        public void ClearEscapeGlobalKeyShortcut()
        {
            if (_hwndSource == null)
            {
                return;
            }

            if (!NativeMethods.UnregisterHotKey(_hwndSource.Handle, _globalHotKeyId))
            {
                Logger.LogWarning("Couldn't unregister the hotkey for Esc.");
            }

            _hwndSource.RemoveHook(ProcessWindowMessages);
        }
    }
}