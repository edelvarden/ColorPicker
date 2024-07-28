using ColorPicker.Helpers;
using ColorPicker.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using static ColorPicker.NativeMethods;

namespace ColorPicker.Keyboard
{
    [Export(typeof(KeyboardMonitor))]
    public class KeyboardMonitor : IDisposable
    {
        private readonly AppStateHandler _appStateHandler;
        private readonly IUserSettings _userSettings;
        private readonly ZoomWindowHelper _zoomWindowHelper;
        private HashSet<int> _currentlyPressedKeys = new HashSet<int>();
        private SortedSet<int> _activationKeys = new SortedSet<int>();
        private GlobalKeyboardHook _keyboardHook;
        private int keyboardMoveSpeed;
        private Key lastArrowKeyPressed = Key.None;

        [ImportingConstructor]
        public KeyboardMonitor(AppStateHandler appStateHandler, IUserSettings userSettings, ZoomWindowHelper zoomWindowHelper)
        {
            _appStateHandler = appStateHandler;
            _userSettings = userSettings;
            _zoomWindowHelper = zoomWindowHelper;
            _userSettings.ActivationShortcut.PropertyChanged += ActivationShortcut_PropertyChanged;
            SetActivationKeys();
        }

        public void Start()
        {
            _keyboardHook = new GlobalKeyboardHook();
            _keyboardHook.KeyboardPressed += Hook_KeyboardPressed;
        }

        private void SetActivationKeys()
        {
            _activationKeys.Clear();

            if (!string.IsNullOrEmpty(_userSettings.ActivationShortcut.Value))
            {
                var keys = _userSettings.ActivationShortcut.Value.Split('+');
                foreach (var key in keys)
                {
                    if (Enum.TryParse(key.Trim(), out Key parsedKey))
                    {
                        _activationKeys.Add(KeyInterop.VirtualKeyFromKey(parsedKey));
                    }
                }
            }
        }

        private void ActivationShortcut_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SetActivationKeys();
        }

        private void Hook_KeyboardPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            var virtualCode = e.KeyboardData.VirtualCode;

            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown || e.KeyboardState == GlobalKeyboardHook.KeyboardState.SysKeyDown)
            {
                _currentlyPressedKeys.Add(virtualCode);
            }
            else if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyUp || e.KeyboardState == GlobalKeyboardHook.KeyboardState.SysKeyUp)
            {
                _currentlyPressedKeys.Remove(virtualCode);
            }

            if (_currentlyPressedKeys.SetEquals(_activationKeys))
            {
                _appStateHandler.ShowColorPicker();
            }

            if (_currentlyPressedKeys.Count == 1 && _currentlyPressedKeys.Contains(27))
            {
                _zoomWindowHelper.CloseZoomWindow();
                _appStateHandler.HideColorPicker();
            }

            // Handle other key events
            if (CheckMoveNeeded(virtualCode, Key.Up, e, 0, -1) ||
                CheckMoveNeeded(virtualCode, Key.Down, e, 0, 1) ||
                CheckMoveNeeded(virtualCode, Key.Left, e, -1, 0) ||
                CheckMoveNeeded(virtualCode, Key.Right, e, 1, 0))
            {
                e.Handled = true;
                return;
            }
        }

        private bool CheckMoveNeeded(int virtualCode, Key key, GlobalKeyboardHookEventArgs e, int xMove, int yMove)
        {
            if (virtualCode == KeyInterop.VirtualKeyFromKey(key))
            {
                if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown && _appStateHandler.IsColorPickerVisible())
                {
                    if (lastArrowKeyPressed == key)
                    {
                        keyboardMoveSpeed++;
                    }
                    else
                    {
                        keyboardMoveSpeed = 1;
                    }

                    lastArrowKeyPressed = key;
                    _appStateHandler.MoveCursor(keyboardMoveSpeed * xMove, keyboardMoveSpeed * yMove);
                    return true;
                }
                else if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyUp)
                {
                    keyboardMoveSpeed = 0;
                    lastArrowKeyPressed = Key.None;
                    return true;
                }
            }

            return false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _keyboardHook?.Dispose();
                _userSettings.ActivationShortcut.PropertyChanged -= ActivationShortcut_PropertyChanged;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
