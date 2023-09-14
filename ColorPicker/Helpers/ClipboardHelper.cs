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
                catch (COMException ex)
                {
                    if ((uint)ex.ErrorCode != CLIPBRD_E_CANT_OPEN)
                    {
                        Logger.LogError("Failed to set text into clipboard", ex);
                    }
                }
                System.Threading.Thread.Sleep(retryDelayMilliseconds);
            }

            // If all retries fail, you may want to throw an exception or log an error here.
            Logger.LogError($"Failed to set text into clipboard after {maxRetries} retries.");
        }
    }
}
