using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using ColorPicker.Helpers;
using static ColorPicker.NativeMethods;

namespace ColorPicker.Mouse
{
    public delegate void MouseUpEventHandler(object sender, System.Drawing.Point p);

    public delegate void SecondaryMouseUpEventHandler(object sender, IntPtr wParam);

    internal class MouseHook
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Interop object")]
        private const int WH_MOUSE_LL = 14;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Interop object")]
        private const int WM_LBUTTONDOWN = 0x0201;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Interop object")]
        private const int WM_LBUTTONUP = 0x0202;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Interop object")]
        private const int WM_MOUSEWHEEL = 0x020A;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Interop object")]
        private const int WM_RBUTTONUP = 0x0205;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Interop object")]
        private const int WM_RBUTTONDOWN = 0x0204;

        private IntPtr _mouseHookHandle;
        private HookProc _mouseDelegate;

        public event MouseUpEventHandler OnLeftMouseDown;
        public event MouseUpEventHandler OnLeftMouseUp;
        public event MouseUpEventHandler OnRightMouseDown;
        private event MouseUpEventHandler MouseDown;
        private event SecondaryMouseUpEventHandler SecondaryMouseUp;
        private event MouseWheelEventHandler MouseWheel;

        public event MouseUpEventHandler OnMouseDown
        {
            add
            {
                Subscribe();
                MouseDown += value;
            }

            remove
            {
                MouseDown -= value;
                Unsubscribe();
            }
        }

        public event SecondaryMouseUpEventHandler OnSecondaryMouseUp
        {
            add
            {
                Subscribe();
                SecondaryMouseUp += value;
            }

            remove
            {
                SecondaryMouseUp -= value;
                Unsubscribe();
            }
        }

        public event MouseWheelEventHandler OnMouseWheel
        {
            add
            {
                Subscribe();
                MouseWheel += value;
            }

            remove
            {
                MouseWheel -= value;
                Unsubscribe();
            }
        }

        private void Unsubscribe()
        {
            if (_mouseHookHandle != IntPtr.Zero)
            {
                var result = UnhookWindowsHookEx(_mouseHookHandle);
                _mouseHookHandle = IntPtr.Zero;
                _mouseDelegate = null;
                if (!result)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    Logger.LogError("Failed to unsubscribe mouse hook with the error code" + errorCode);
                }
            }
        }

        private void Subscribe()
        {
            if (_mouseHookHandle == IntPtr.Zero)
            {
                _mouseDelegate = MouseHookProc;
                _mouseHookHandle = SetWindowsHookEx(
                    WH_MOUSE_LL,
                    _mouseDelegate,
                    GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName),
                    0);

                if (_mouseHookHandle == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    Logger.LogError("Failed to subscribe mouse hook with the error code" + errorCode);
                }
            }
        }

        private IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MSLLHOOKSTRUCT mouseHookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                if (wParam.ToInt32() == WM_LBUTTONDOWN)
                {
                    OnLeftMouseDown?.Invoke(null, new System.Drawing.Point(mouseHookStruct.pt.x, mouseHookStruct.pt.y));
                    return new IntPtr(-1);
                }

                if (wParam.ToInt32() == WM_LBUTTONUP)
                {
                    OnLeftMouseUp?.Invoke(null, new System.Drawing.Point(mouseHookStruct.pt.x, mouseHookStruct.pt.y));
                    return new IntPtr(-1);
                }

                if (wParam.ToInt32() == WM_RBUTTONUP)
                {
                    SecondaryMouseUp?.Invoke(null, wParam);
                    return new IntPtr(-1);
                }

                if (wParam.ToInt32() == WM_RBUTTONDOWN)
                {
                    OnRightMouseDown?.Invoke(null, new System.Drawing.Point(mouseHookStruct.pt.x, mouseHookStruct.pt.y));
                    return new IntPtr(-1);
                }

                if (wParam.ToInt32() == WM_MOUSEWHEEL)
                {
                    if (MouseWheel != null)
                    {
                        MouseDevice mouseDev = InputManager.Current.PrimaryMouseDevice;
                        MouseWheel.Invoke(null, new MouseWheelEventArgs(mouseDev, Environment.TickCount, (int)mouseHookStruct.mouseData >> 16));
                        return new IntPtr(-1);
                    }
                }
            }

            return CallNextHookEx(_mouseHookHandle, nCode, wParam, lParam);
        }
    }

}
