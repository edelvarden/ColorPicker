using ColorPicker.Helpers;
using ColorPicker.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;

namespace ColorPicker.Keyboard
{
    [Export(typeof(KeyboardMonitor))]
    public class KeyboardMonitor
    {
        private readonly AppStateHandler _appStateHandler;
        private readonly IUserSettings _userSettings;
        private readonly ZoomWindowHelper _zoomWindowHelper;
        private HashSet<int> _currentlyPressedKeys = new HashSet<int>();
        private SortedSet<int> _activationKeys = new SortedSet<int>();
        private GlobalKeyboardHook _keyboardHook;

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
        }
    }
}
