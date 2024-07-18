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
        private List<string> _previouslyPressedKeys = new List<string>();
        private List<string> _activationKeys = new List<string>();
        private GlobalKeyboardHook _keyboardHook;
        private bool _activationShortcutPressed;
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
                    _activationKeys.Add(key.Trim());
                }

                _activationKeys.Sort();
            }
        }

        private void ActivationShortcut_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SetActivationKeys();
        }

        private void Hook_KeyboardPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            var currentlyPressedKeys = new List<string>();
            var virtualCode = e.KeyboardData.VirtualCode;

            // ESC pressed
            if (virtualCode == KeyInterop.VirtualKeyFromKey(Key.Escape) && e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                if (_appStateHandler.IsColorPickerVisible())
                {
                    _appStateHandler.HideColorPicker();
                    e.Handled = true;
                    return;
                }
            }

            if (CheckMoveNeeded(virtualCode, Key.Up, e, 0, -1))
            {
                e.Handled = true;
                return;
            }
            else if (CheckMoveNeeded(virtualCode, Key.Down, e, 0, 1))
            {
                e.Handled = true;
                return;
            }
            else if (CheckMoveNeeded(virtualCode, Key.Left, e, -1, 0))
            {
                e.Handled = true;
                return;
            }
            else if (CheckMoveNeeded(virtualCode, Key.Right, e, 1, 0))
            {
                e.Handled = true;
                return;
            }

            var name = Helper.GetKeyName((uint)virtualCode);

            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown || e.KeyboardState == GlobalKeyboardHook.KeyboardState.SysKeyDown)
            {
                AddModifierKeys(currentlyPressedKeys);
                currentlyPressedKeys.Add(name);
            }

            currentlyPressedKeys.Sort();

            if (currentlyPressedKeys.Count == 0 && _previouslyPressedKeys.Count != 0)
            {
                _activationShortcutPressed = false;
            }

            _previouslyPressedKeys = currentlyPressedKeys;

            if (ArraysAreSame(currentlyPressedKeys, _activationKeys))
            {
                if (!_activationShortcutPressed)
                {
                    _activationShortcutPressed = true;
                    _appStateHandler.ShowColorPicker();
                }
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
                    lastArrowKeyPressed = Key.None;
                    keyboardMoveSpeed = 0;
                }
            }

            return false;
        }

        private static bool ArraysAreSame(List<string> first, List<string> second)
        {
            if (first.Count != second.Count || (first.Count == 0 && second.Count == 0))
            {
                return false;
            }

            for (int i = 0; i < first.Count; i++)
            {
                if (first[i] != second[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static void AddModifierKeys(List<string> currentlyPressedKeys)
        {
            if ((GetAsyncKeyState(VK_SHIFT) & 0x8000) != 0)
            {
                currentlyPressedKeys.Add("Shift");
            }

            if ((GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0)
            {
                currentlyPressedKeys.Add("Ctrl");
            }

            if ((GetAsyncKeyState(VK_MENU) & 0x8000) != 0)
            {
                currentlyPressedKeys.Add("Alt");
            }

            if ((GetAsyncKeyState(VK_LWIN) & 0x8000) != 0 || (GetAsyncKeyState(VK_RWIN) & 0x8000) != 0)
            {
                currentlyPressedKeys.Add("Win");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            _keyboardHook?.Dispose();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
