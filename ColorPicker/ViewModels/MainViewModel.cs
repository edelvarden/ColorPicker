using ColorMeter.Helpers;
using ColorName;
using ColorPicker.Common;
using ColorPicker.Helpers;
using ColorPicker.Keyboard;
using ColorPicker.Mouse;
using ColorPicker.Settings;
using ColorPicker.ViewModelContracts;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ColorPicker.ViewModels
{
    [Export(typeof(IMainViewModel))]
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        private string _colorString;
        private Brush _displayedColorBrush;
        private readonly IMouseInfoProvider _mouseInfoProvider;
        private readonly ZoomWindowHelper _zoomWindowHelper;
        private readonly AppStateHandler _appStateHandler;
        private readonly IColorProvider _colorProvider;
        private readonly IUserSettings _userSettings;
        private System.Drawing.Color _currentColor;
        private bool _mouseDown;

        [ImportingConstructor]
        public MainViewModel(
            IMouseInfoProvider mouseInfoProvider,
            ZoomWindowHelper zoomWindowHelper,
            AppStateHandler appStateHandler,
            KeyboardMonitor keyboardMonitor,
            IUserSettings userSettings,
            IColorProvider colorProvider)
        {
            _mouseInfoProvider = mouseInfoProvider;
            _zoomWindowHelper = zoomWindowHelper;
            _appStateHandler = appStateHandler;
            _userSettings = userSettings;
            _colorProvider = colorProvider;
            mouseInfoProvider.MouseColorChanged += Mouse_ColorChanged;
            mouseInfoProvider.OnLeftMouseDown += MouseInfoProvider_OnLeftMouseDown;
            mouseInfoProvider.OnLeftMouseUp += MouseInfoProvider_OnLeftMouseUp;
            mouseInfoProvider.OnRightMouseDown += MouseInfoProvider_OnRightMouseDown;
            mouseInfoProvider.MousePositionChanged += MouseInfoProvider_MousePositionChanged;
            mouseInfoProvider.OnMouseWheel += MouseInfoProvider_OnMouseWheel;

            keyboardMonitor.Start();
        }

        public string ColorString
        {
            get
            {
                if (_userSettings.ShowColorName.Value)
                {
                    return $"{_colorString} - {ColorNameProvider.GetColorNameFromRGB(_currentColor.R, _currentColor.G, _currentColor.B).colorName}";
                }
                return _colorString;
            }
            set
            {
                _colorString = value;
                OnPropertyChanged();
            }
        }

        public Brush DisplayedColorBrush
        {
            get
            {
                return _displayedColorBrush;
            }
            private set
            {
                _displayedColorBrush = value;
                OnPropertyChanged();
            }
        }

        private void Mouse_ColorChanged(object sender, System.Drawing.Color color)
        {
            _currentColor = color;
            ColorString = ColorFormatHelper.ColorToString(color, _userSettings.SelectedColorFormat.Value);
            DisplayedColorBrush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        private void MouseInfoProvider_MousePositionChanged(object sender, Point e)
        {
            // show meter area only after we detected a movement
            if (_mouseDown && !_appStateHandler.IsMeterAreaShown)
            {
                _appStateHandler.ShowMeterArea();
            }
            if (!_mouseDown && !_appStateHandler.IsMeterAreaShown)
            {
                _currentColor = _colorProvider.GetPixelColor(e);
                ColorString = ColorFormatHelper.ColorToString(_currentColor, _userSettings.SelectedColorFormat.Value);
                DisplayedColorBrush = new SolidColorBrush(Color.FromArgb(_currentColor.A, _currentColor.R, _currentColor.G, _currentColor.B));
            }
        }

        private void MouseInfoProvider_OnLeftMouseDown(object sender, System.Drawing.Point p)
        {
            _appStateHandler.HideColorPicker();
            _appStateHandler.HideMeterArea();
            _mouseDown = true;
        }

        private void MouseInfoProvider_OnLeftMouseUp(object sender, System.Drawing.Point p)
        {
            if (ColorString != null)
            {
                ClipboardHelper.CopyIntoClipboard(ColorString);
            }

            _mouseDown = false;
            _mouseInfoProvider.StopMonitoring();
        }

        private void MouseInfoProvider_OnRightMouseDown(object sender, System.Drawing.Point p)
        {
            _appStateHandler.ShowColorHistory();
        }

        private void MouseInfoProvider_OnMouseWheel(object sender, Tuple<Point, bool> e)
        {
            _zoomWindowHelper.Zoom(e.Item1, e.Item2);
        }
    }
}
