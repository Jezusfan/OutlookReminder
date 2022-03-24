using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OutlookReminder
{
    internal class WindowHook: IDisposable
    {
        private readonly Action _windowsClosed;

        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        //private WinEventDelegate _winEventHookHandler;
        private readonly uint _processId;

        #region Win32 API

        private const int WINEVENT_INCONTEXT = 4;
        private const int WINEVENT_OUTOFCONTEXT = 0;
        private const int WINEVENT_SKIPOWNPROCESS = 2;
        private const int WINEVENT_SKIPOWNTHREAD = 1;


        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_SHOWWINDOW = 0x0040;

        [StructLayout(LayoutKind.Sequential)]
        public struct WindowRect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        //WARN: Only for "Any CPU":
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr handle, out uint processId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out WindowRect lpRect);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();

        [DllImport("User32.dll", SetLastError = true)]
        private static extern IntPtr SetWinEventHook(
            uint eventMin,
            uint eventMax,
            IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc,
            uint idProcess,
            uint idThread,
            int dwFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        enum SystemEvents
        {
            EVENT_SYSTEM_DESTROY = 0x8001,
            EVENT_SYSTEM_MINIMIZESTART = 0x0016,
            EVENT_SYSTEM_MINIMIZEEND = 0x0017,
            EVENT_SYSTEM_FOREGROUND = 0x0003
        }

        #endregion

        public WindowHook(int processId, Action windowsClosedDelegate)
        {
            _windowsClosed = windowsClosedDelegate;
            _processId = Convert.ToUInt32(processId);
            //_winEventHookHandler = this.WinEvent;
            //GC.KeepAlive(_winEventHookHandler);
        }

        public static void FocusWindow(IntPtr handle)
        {
            SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
            //SetForegroundWindow(handle);
        }

        public void AddWindowCloseHandler(IntPtr windowhandle)
        {
            //we start a background thread monitoring the visibility of the window
            //cause hooking into the closing/minimizing event of the window does not seem to work
            Task.Factory.StartNew((i) =>
            {
                IntPtr handle = (IntPtr)i;
                do
                {
                    //uint threadid = GetWindowThreadProcessId(window.Handle, out processid);
                    Thread.Sleep(50);
                } while (handle != IntPtr.Zero && IsWindowVisible(handle));
                if (_windowsClosed != null)
                    _windowsClosed();
            }, windowhandle);

            //if (_monitorWindows.TryAdd(info.Handle, info))
            //{
            //    info.WindowEventHook = SetWinEventHook((uint)SystemEvents.EVENT_SYSTEM_MINIMIZESTART, (uint)SystemEvents.EVENT_SYSTEM_DESTROY, IntPtr.Zero, _winEventHookHandler, _processId, 0, WINEVENT_OUTOFCONTEXT);
            //    if (IntPtr.Zero.Equals(info.WindowEventHook)) throw new Win32Exception("Unable to listen to foreground window changes");
            //}
            
        }


        public List<WindowInfo> GetChildWindows()
        {
            IntPtr shellWindow = GetShellWindow();
            var windows = new List<WindowInfo>();
            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;
                uint proc;
                uint threadId = GetWindowThreadProcessId(hWnd, out proc);
                if (_processId == proc)
                {
                    WindowRect rectangle;
                    if (GetWindowRect(hWnd, out rectangle))
                    {
                        int length = GetWindowTextLength(hWnd);
                        if (length == 0) return true;
                        StringBuilder builder = new StringBuilder(length);
                        GetWindowText(hWnd, builder, length + 1);
                        windows.Add(new WindowInfo {Caption = builder.ToString(), Handle = hWnd, Rectangle = rectangle, Thread = threadId });
                    }
                }
                return true;

            }, 0);
            return windows;
        }

        /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
        /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
        public static IDictionary<IntPtr, string> GetOpenWindows()
        {
            IntPtr shellWindow = GetShellWindow();
            Dictionary<IntPtr, string> windows = new Dictionary<IntPtr, string>();

            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;

            }, 0);

            return windows;
        }


        //private void WinEvent(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        //{
        //    WindowInfo window;
        //    switch (eventType)
        //    {
        //        case (uint)SystemEvents.EVENT_SYSTEM_DESTROY:

        //            //if we want something to happen when this window is closed, we execute that now
        //            if (_monitorWindows.TryGetValue(hWnd, out window))
        //                HandleFormClosed(window);
        //            break;
        //        case (uint)SystemEvents.EVENT_SYSTEM_MINIMIZEEND:
        //            //if we want something to happen when this window is closed, we execute that now
        //            if (_monitorWindows.TryGetValue(hWnd, out window))
        //                HandleFormClosed(window);
        //            break;
        //            //extend here when required
        //    }
        //}

        //void HandleFormClosed(WindowInfo window)
        //{
        //    if (window.CloseAction != null)
        //        window.CloseAction();
        //    // get rid of managed resources
        //    if (!IntPtr.Zero.Equals(window.WindowEventHook)) UnhookWinEvent(window.WindowEventHook);
        //}

        public class WindowInfo
        {
            public WindowRect Rectangle { get; set; }
            public IntPtr Handle { get; set; }
            public uint Thread { get; set; }
            public string Caption { get; set; }
            public Action CloseAction { get; set; }

        }


        public void Dispose()
        {
            Dispose(true);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //_winEventHookHandler = null;
            }
            // get rid of unmanaged resources
        }
        

    }




}
