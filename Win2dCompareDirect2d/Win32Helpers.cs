using SharpGen.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Win2dCompareDirect2d;

public static class Win32Helpers
{


    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr CreateEvent(
        IntPtr lpEventAttributes, bool bManualReset,
        bool bInitialState, string lpName);

    [DllImport("kernel32.dll")]
    public static extern bool SetEvent(IntPtr hEvent);

    [DllImport("ole32.dll")]
    public static extern uint CoWaitForMultipleObjects(
        uint dwFlags, uint dwMilliseconds, ulong nHandles,
        IntPtr[] pHandles, out uint dwIndex);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetParent(IntPtr child, IntPtr parent);


    [DllImport("user32.dll")]
    public static extern IntPtr GetDesktopWindow();

    [Flags]
    public enum SendMessageTimeoutFlags : uint
    {
        SMTO_NORMAL = 0x0000,
        SMTO_BLOCK = 0x0001,
        SMTO_ABORTIFHUNG = 0x0002,
        SMTO_NOTIMEOUTIFNOTHUNG = 0x0008,
        SMTO_ERRORONEXIT = 0x0020
    }

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam,
        SendMessageTimeoutFlags fuFlags, uint uTimeout, out IntPtr lpdwResult);

    public enum DWMWINDOWATTRIBUTE
    {
        DWMWA_NCRENDERING_ENABLED = 1,
        DWMWA_NCRENDERING_POLICY,
        DWMWA_TRANSITIONS_FORCEDISABLED,
        DWMWA_ALLOW_NCPAINT,
        DWMWA_CAPTION_BUTTON_BOUNDS,
        DWMWA_NONCLIENT_RTL_LAYOUT,
        DWMWA_FORCE_ICONIC_REPRESENTATION,
        DWMWA_FLIP3D_POLICY,
        DWMWA_EXTENDED_FRAME_BOUNDS,
        DWMWA_HAS_ICONIC_BITMAP,
        DWMWA_DISALLOW_PEEK,
        DWMWA_EXCLUDED_FROM_PEEK,
        DWMWA_CLOAK,
        DWMWA_CLOAKED,
        DWMWA_FREEZE_REPRESENTATION,
        DWMWA_PASSIVE_UPDATE_MODE,
        DWMWA_USE_HOSTBACKDROPBRUSH,
        DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
        DWMWA_WINDOW_CORNER_PREFERENCE = 33,
        DWMWA_BORDER_COLOR,
        DWMWA_CAPTION_COLOR,
        DWMWA_TEXT_COLOR,
        DWMWA_VISIBLE_FRAME_BORDER_THICKNESS,
        DWMWA_SYSTEMBACKDROP_TYPE,
        DWMWA_LAST
    };

    public enum DWM_WINDOW_CORNER_PREFERENCE
    {
        DWMWCP_DEFAULT = 0,
        DWMWCP_DONOTROUND = 1,
        DWMWCP_ROUND = 2,
        DWMWCP_ROUNDSMALL = 3
    }

    [DllImport("Dwmapi.dll", SetLastError = true, CharSet = CharSet.Unicode, PreserveSig = false)]
    public static extern IntPtr DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute,
        ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute, uint cbAttribute);

    [DllImport("Dwmapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute,
        int cbAttribute);


    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

    public const int RDW_INVALIDATE = 0x0001;
    public const int RDW_INTERNALPAINT = 0x0002;
    public const int RDW_ERASE = 0x0004;

    public const int RDW_VALIDATE = 0x0008;
    public const int RDW_NOINTERNALPAINT = 0x0010;
    public const int RDW_NOERASE = 0x0020;

    public const int RDW_NOCHILDREN = 0x0040;
    public const int RDW_ALLCHILDREN = 0x0080;

    public const int RDW_UPDATENOW = 0x0100;
    public const int RDW_ERASENOW = 0x0200;

    public const int RDW_FRAME = 0x0400;
    public const int RDW_NOFRAME = 0x0800;


    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass,
        string lpszWindow);


    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
    private static extern nint IntSetWindowLongPtr(nint hWnd, int nIndex, nint dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
    private static extern int IntSetWindowLong(nint hWnd, int nIndex, int dwNewLong);

    private static int IntPtrToInt32(nint intPtr)
    {
        return unchecked((int)intPtr.ToInt64());
    }

    [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
    public static extern void SetLastError(int dwErrorCode);

    [Flags]
    public enum ExtendedWindowStyles
    {
        // ...
        WS_EX_TOOLWINDOW = 0x00000080,

        WS_EX_TRANSPARENT = 32
        // ...
    }

    [DllImport("User32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
    public static extern long GetWindowLong32(nint hWnd, int nIndex);

    [DllImport("User32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
    public static extern long GetWindowLongPtr64(nint hWnd, int nIndex);

    public const int GWL_STYLE = -16;
    public const int GWL_EXSTYLE = -20;
    public const int WS_EX_LAYERED = 0x00080000;
    public const int WS_EX_TRANSPARENT = 0x00000020;

    public static long GetWindowLong(nint hWnd, int nIndex)
    {
        if (nint.Size == 4)
        {
            return GetWindowLong32(hWnd, nIndex);
        }

        return GetWindowLongPtr64(hWnd, nIndex);
    }

    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool SetLayeredWindowAttributes(nint hwnd, uint crKey, byte bAlpha, uint dwFlags);

    public static nint SetWindowLong(nint hWnd, int nIndex, nint dwNewLong)
    {
        int error = 0;
        nint result = nint.Zero;
        // Win32 SetWindowLong doesn't clear error on success
        SetLastError(0);

        if (nint.Size == 4)
        {
            // use SetWindowLong
            int tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
            error = Marshal.GetLastWin32Error();
            result = new nint(tempResult);
        }
        else
        {
            // use SetWindowLongPtr
            result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
            error = Marshal.GetLastWin32Error();
        }

        if (result == nint.Zero && error != 0)
        {
            throw new System.ComponentModel.Win32Exception(error);
        }

        return result;
    }

    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern uint GetDpiForWindow(IntPtr hwnd);


    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern uint RegisterWindowMessage(string lpString);

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    public static extern bool Shell_NotifyIcon(uint dwMessage, [In] ref NOTIFYICONDATA lpData);

    public const uint NIM_ADD = 0x00000000;
    public const uint NIM_MODIFY = 0x00000001;
    public const uint NIM_DELETE = 0x00000002;
    public const uint NIF_MESSAGE = 0x00000001;
    public const uint NIF_ICON = 0x00000002;
    public const uint NIF_TIP = 0x00000004;

    public const int WM_USER = 0x0400;
    public const int WM_TRAYICON = WM_USER + 1;

    public const uint IMAGE_ICON = 1;
    public const uint LR_LOADFROMFILE = 0x00000010;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct NOTIFYICONDATA
    {
        public uint cbSize;
        public IntPtr hWnd;
        public uint uID;
        public uint uFlags;
        public uint uCallbackMessage;
        public IntPtr hIcon;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szTip;

        public uint dwState;
        public uint dwStateMask;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szInfo;

        public uint uTimeoutOrVersion;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szInfoTitle;

        public uint dwInfoFlags;
        public Guid guidItem;
        public IntPtr hBalloonIcon;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr LoadImage(IntPtr hInst, string lpszName, uint uType, int cxDesired, int cyDesired,
        uint fuLoad);


    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy,
        uint uFlags);

    public const uint LWA_COLORKEY = 0x00000001;
    public const uint LWA_ALPHA = 0x00000002;
    public const uint SWP_FRAMECHANGED = 0x0020;
    public const uint SWP_NOMOVE = 0x0002;
    public const uint SWP_NOSIZE = 0x0001;
    public const uint SWP_NOZORDER = 0x0004;
    public const uint SWP_SHOWWINDOW = 0x0040;


    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    public const int SW_SHOW = 5;


    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, IntPtr pptDst, IntPtr psize,
        IntPtr hdcSrc, IntPtr pprSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);

    [DllImport("gdi32.dll", SetLastError = true)]
    public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    [DllImport("gdi32.dll", SetLastError = true)]
    public static extern bool DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll", SetLastError = true)]
    public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);


    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SIZE
    {
        public int cx;
        public int cy;

        public SIZE(int x, int y)
        {
            cx = x;
            cy = y;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BLENDFUNCTION
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

    public const int ULW_ALPHA = 0x00000002;
    public const byte AC_SRC_OVER = 0x00;
    public const byte AC_SRC_ALPHA = 0x01;

    [DllImport("gdi32.dll")]
    static extern bool DeleteObject(IntPtr hObject);


    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct BITMAP
    {
        public int bmType;
        public int bmWidth;
        public int bmHeight;
        public int bmWidthBytes;
        public short bmPlanes;
        public short bmBitsPixel;
        public IntPtr bmBits;
    }

    [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern int GetObject(IntPtr hFont, int nSize, out BITMAP bm);


    public const int ULW_COLORKEY = 0x00000001;
    public const int ULW_OPAQUE = 0x00000004;

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public RECT(int Left, int Top, int Right, int Bottom)
        {
            left = Left;
            top = Top;
            right = Right;
            bottom = Bottom;
        }
    }

    [DllImport("User32.dll", SetLastError = true)]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);


    public static int HIWORD(int n)
    {
        return (n >> 16) & 0xffff;
    }
    public static int LOWORD(int n)
    {
        return n & 0xffff;
    }

    public delegate int SUBCLASSPROC(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData);


    [DllImport("Comctl32.dll", SetLastError = true)]
    public static extern bool SetWindowSubclass(IntPtr hWnd, SUBCLASSPROC pfnSubclass, uint uIdSubclass, uint dwRefData);

    [DllImport("Comctl32.dll", SetLastError = true)]
    public static extern int DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

    public const int WM_CONTEXTMENU = 0x007B;
    public const int WM_RBUTTONDOWN = 0x0204;
    public const int WM_RBUTTONUP = 0x0205;
    public const int WM_ENTERMENULOOP = 0x0211;
    public const int WM_EXITMENULOOP = 0x0212;
    public const int WM_INITMENUPOPUP = 0x0117;
    public const int WM_UNINITMENUPOPUP = 0x0125;

    public const int WM_DRAWITEM = 0x002B;
    public const int WM_MEASUREITEM = 0x002C;

    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool GetCursorPos(out Windows.Graphics.PointInt32 lpPoint);

    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr CreatePopupMenu();

    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern int GetMenuItemCount(IntPtr hMenu);

    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool DestroyMenu(IntPtr hMenu);

    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool AppendMenu(IntPtr hMenu, uint uFlags, IntPtr uIDNewItem, string lpNewItem);

    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool ModifyMenu(IntPtr hMnu, uint uPosition, uint uFlags, IntPtr uIDNewItem, IntPtr lpNewItem);

    public const int MF_STRING = 0x00000000;
    public const int MF_BITMAP = 0x00000004;
    public const int MF_OWNERDRAW = 0x00000100;

    public const int MF_POPUP = 0x00000010;
    public const int MF_MENUBARBREAK = 0x00000020;
    public const int MF_MENUBREAK = 0x00000040;
    public const int MF_SEPARATOR = 0x00000800;

    public const int MF_BYCOMMAND = 0x00000000;
    public const int MF_BYPOSITION = 0x00000400;

    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern uint TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

    public const int TPM_LEFTBUTTON = 0x0000;
    public const int TPM_RIGHTBUTTON = 0x0002;
    public const int TPM_LEFTALIGN = 0x0000;
    public const int TPM_CENTERALIGN = 0x0004;
    public const int TPM_RIGHTALIGN = 0x0008;
    public const int TPM_TOPALIGN = 0x0000;
    public const int TPM_VCENTERALIGN = 0x0010;
    public const int TPM_BOTTOMALIGN = 0x0020;
    public const int TPM_HORIZONTAL = 0x0000;     /* Horz alignment matters more */
    public const int TPM_VERTICAL = 0x0040;     /* Vert alignment matters more */
    public const int TPM_NONOTIFY = 0x0080;     /* Don't send any notification msgs */
    public const int TPM_RETURNCMD = 0x0100;
    public const int TPM_RECURSE = 0x0001;
    public const int TPM_HORPOSANIMATION = 0x0400;
    public const int TPM_HORNEGANIMATION = 0x0800;
    public const int TPM_VERPOSANIMATION = 0x1000;
    public const int TPM_VERNEGANIMATION = 0x2000;
    public const int TPM_NOANIMATION = 0x4000;
    public const int TPM_LAYOUTRTL = 0x8000;
    public const int TPM_WORKAREA = 0x10000;

    public const int WM_TRAYMOUSEMESSAGE = WM_USER + 1024;
}