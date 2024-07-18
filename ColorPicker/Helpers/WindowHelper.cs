using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace ColorPicker.Helpers
{
    internal static class WindowHelper
    {
        internal static void SetPositionAndSize(this Window window, double left, double top, double width, double height)
        {
            var helper = new WindowInteropHelper(window);

            int pxLeft, pxTop;
            TransformToPixels(window, left, top, out pxLeft, out pxTop);

            int pxWidth, pxHeight;
            TransformToPixels(window, width, height, out pxWidth, out pxHeight);

            if (!MoveWindow(helper.Handle, pxLeft, pxTop, pxWidth, pxHeight, true))
            {
                // Handle error (optional)
                throw new InvalidOperationException("Failed to move the window.");
            }
        }

        internal static void TransformToPixels(Visual visual, double unitX, double unitY, out int pixelX, out int pixelY)
        {
            Matrix matrix = GetTransformToDeviceMatrix(visual);

            pixelX = (int)(matrix.M11 * unitX);
            pixelY = (int)(matrix.M22 * unitY);
        }

        private static Matrix GetTransformToDeviceMatrix(Visual visual)
        {
            var source = PresentationSource.FromVisual(visual);
            if (source != null)
            {
                return source.CompositionTarget.TransformToDevice;
            }

            using (var src = new HwndSource(new HwndSourceParameters()))
            {
                return src.CompositionTarget.TransformToDevice;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, [MarshalAs(UnmanagedType.Bool)] bool bRepaint);
    }
}
