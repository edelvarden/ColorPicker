using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace ColorPicker.Helpers
{
    public static class ClipboardHelper
    {
        private const uint CLIPBRD_E_CANT_OPEN = 0x800401D0;

        public static void CopyIntoClipboard(string value, int maxRetries = 10, int retryDelayMilliseconds = 10)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    Clipboard.SetDataObject(value.ToLowerInvariant());
                    return; // Success, exit the loop
                }
                catch (COMException ex) when ((uint)ex.ErrorCode == CLIPBRD_E_CANT_OPEN)
                {
                    var retriesLeft = maxRetries - i - 1;
                    if (retriesLeft > 0)
                    {
                        Logger.LogWarning($"Failed to set text into clipboard, retrying {retriesLeft} more times.");
                        System.Threading.Thread.Sleep(retryDelayMilliseconds);
                    }
                    else
                    {
                        Logger.LogError($"Failed to set text into clipboard after {maxRetries} retries.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Unexpected error while setting text into clipboard", ex);
                    // Consider whether to rethrow or handle other exceptions here
                }
            }
        }
    }
}
