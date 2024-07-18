using ColorPicker.Settings;
using System.Windows.Input;

namespace ColorPicker.ViewModelContracts
{
    public interface ISettingsViewModel
    {
        bool RunOnStartup { get; set; }

        bool ShowingKeyboardCaptureOverlay { get; set; }

        bool ShowColorName { get; set; }

        string ShortCut { get; }

        string ShortCutPreview { get; set; }

        string ApplicationVersion { get; }

        ColorFormat SelectedColorFormat { get; }

        ICommand ChangeShortcutCommand { get; }

        ICommand ResetShortcutCommand { get; }

        ICommand ConfirmShortcutCommand { get; }

        ICommand CancelShortcutCommand { get; }
    }
}
