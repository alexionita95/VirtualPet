using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VirtualPet
{

    /// <summary>
    /// Native methods for the windows detection functionality. User32.dll is used for this.
    /// </summary>
    internal static class NativeMethods
    {

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsZoomed(IntPtr hWnd);

        /// <summary>
        /// Get size of a window.
        /// </summary>
        /// <param name="hWnd">Handle to window.</param>
        /// <param name="lpRect">returns the size of the window.</param>
        /// <returns>True if successfully.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);

        /// <summary>
        /// Get a list of all windows present on the desktop.
        /// </summary>
        /// <param name="enumFunc">Enumeration function.</param>
        /// <param name="lParam">User defined value.</param>
        /// <returns>True if successfully.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnumWindows(EnumWindowsProc enumFunc, IntPtr lParam);

        /// <summary>
        /// If window is visible (is on the desktop).
        /// </summary>
        /// <param name="hWnd">Handle to the window.</param>
        /// <returns>True if successfully.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindowVisible(IntPtr hWnd);

        /// <summary>
        /// Get the text present in the window title bar.
        /// </summary>
        /// <param name="hWnd">Handle to the window.</param>
        /// <param name="lpString">Array, where the title should be copied.</param>
        /// <param name="nMaxCount">Array size.</param>
        /// <returns>Length of the title on the title bar.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        /// <summary>
        /// Get the values of the title bar from the window.
        /// </summary>
        /// <param name="hWnd">Handle to the window.</param>
        /// <param name="pti">Pointer to a valid structure. Will be filled with all information.</param>
        /// <returns>True if successfully.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetTitleBarInfo(IntPtr hWnd, ref TITLEBARINFO pti);

        /// <summary>
        /// Change window modality (show, normal, hidden, maximize, ...) of a window.
        /// </summary>
        /// <param name="hWnd">Handle to the window.</param>
        /// <param name="nCmdShow">Command to change modality.</param>
        /// <returns>True if successfully</returns>
        /// <seealso cref="ShowWindow(IntPtr, int)"/>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// Set the focus to the window and bring it to foreground. Used once the pet is felt over it.
        /// </summary>
        /// <param name="hWnd">Handle to the window.</param>
        /// <returns>True</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Get the next window on the desktop (next to the user in Z-order, child, first window, etc.)
        /// </summary>
        /// <param name="hWnd">Handle to the current window.</param>
        /// <param name="nCmdShow">Command of the next window to get, <see cref="GetWindow(IntPtr, int)"/></param>
        /// <returns>Pointer to the next window.</returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// Get the window on the top, if hWnd is NULL, the top in Z-order will be returned
        /// </summary>
        /// <param name="hWnd">Handle to the current window.</param>
        /// <returns>Pointer to the next window.</returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr GetTopWindow(IntPtr hWnd);

        /// <summary>
        /// Get the desktop window.
        /// </summary>
        /// <returns>Pointer to the first window.</returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        internal static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, StringBuilder slpClass, StringBuilder slpWindow);

        /// <summary>
        /// Structure with the information about the title bar of the window.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct TITLEBARINFO
        {
            /// <summary>
            /// Size (in bytes) of the current structure.
            /// </summary>
            public int cbSize;
            /// <summary>
            /// Dimension of the title bar.
            /// </summary>
            public RECT rcTitleBar;
            /// <summary>
            /// 6 bytes containing the states of the title bar.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public int[] rgstate;
        }

        /// <summary>
        /// Dimension structure (used for the windows size).
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            /// <summary>
            /// x position of upper-left corner
            /// </summary>
            public int Left;
            /// <summary>
            /// y position of upper-left corner
            /// </summary>
            public int Top;
            /// <summary>
            /// x position of lower-right corner
            /// </summary>
            public int Right;
            /// <summary>
            /// y position of lower-right corner
            /// </summary>
            public int Bottom;
        }

        /// <summary>
        /// Procedure used to find all windows on the desktop.
        /// </summary>
        /// <param name="hWnd">Handle of the current found window.</param>
        /// <param name="lParam">User defined parameter.</param>
        /// <returns>True if successfully found another window.</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        internal delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);
    }

}
