using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Ruminate.GUI.Framework
{
    public class InputHook
    {
        #region Windows function imports

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        #endregion

        #region Windows constants        

        // ReSharper disable InconsistentNaming
#pragma warning disable 169
        private const int WH_MOUSE_LL = 14;
        private const int WH_KEYBOARD_LL = 13;

        private const int WH_MOUSE = 7;
        private const int WH_KEYBOARD = 2;

        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_MOUSEHOVER = 0x2A1;

        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_XBUTTONDOWN = 0x20B;

        private const int WM_LBUTTONUP = 0x202;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_XBUTTONUP = 0x20C;

        private const int WM_LBUTTONDBLCLK = 0x203;
        private const int WM_RBUTTONDBLCLK = 0x206;
        private const int WM_MBUTTONDBLCLK = 0x209;
        private const int WM_XBUTTONDBLCLK = 0x20D;

        private const int WM_MOUSEWHEEL = 0x020A;

        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_CHAR = 0x102;

        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;

        private const byte VK_SHIFT = 0x10;
        private const byte VK_CAPITAL = 0x14;
        private const byte VK_NUMLOCK = 0x90;

        private const int GWL_WNDPROC = -4;
        private const int WM_IME_SETCONTEXT = 0x281;
        private const int WM_INPUTLANGCHANGE = 0x51;
        private const int WM_GETDLGCODE = 0x87;
        private const int WM_IME_COMPOSITION = 0x10F;
        private const int DLGC_WANTALLKEYS = 4;
#pragma warning restore 169
        // ReSharper restore InconsistentNaming

        #endregion        

        #region Events

        /// <summary>Event raised when a character has been entered.</summary>		
        public event CharEnteredHandler CharEntered;

        /// <summary>Event raised when a key has been pressed down. May fire multiple times due to keyboard repeat.</summary>		
        public event KeyEventHandler KeyDown;

        /// <summary>Event raised when a key has been released.</summary>
        public event KeyEventHandler KeyUp;

        /// <summary>Event raised when a mouse button is pressed.</summary>
        public event MouseEventHandler MouseDown;

        /// <summary>Event raised when a mouse button is released.</summary>	
        public event MouseEventHandler MouseUp;

        /// <summary>Event raised when the mouse changes location.</summary>
        public event MouseEventHandler MouseMove;

        /// <summary>Event raised when the mouse has hovered in the same location for a short period of time.</summary>		
        public event MouseEventHandler MouseHover;

        /// <summary>Event raised when the mouse wheel has been moved.</summary>	
        public event MouseEventHandler MouseWheel;

        /// <summary>Event raised when a mouse button has been double clicked.</summary>	
        public event MouseEventHandler MouseDoubleClick;

        #endregion

        private Game.Game1 _game;

        public static Point MouseLocation {
            get {
                var state = Mouse.GetState();
                return new Point(state.X, state.Y);
            }
        }

        public InputHook(Game.Game1 game, bool installMouseHook = true, bool installKeyboardHook = true)
        {
            _game = game;
            Start(installMouseHook, installKeyboardHook);
        }

        ~InputHook()
        {
            Stop(true, true, false);
        }

        private int _hMouseHook;
        private int _hKeyboardHook;

        private static HookProc _mouseHookProcedure;
        private static HookProc _keyboardHookProcedure;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        private void Start(bool installMouseHook, bool installKeyboardHook)
        {
            // install Mouse hook only if it is not installed and must be installed
            if (_hMouseHook == 0 && installMouseHook)
            {
                _mouseHookProcedure = MouseHookProc;
                _hMouseHook = SetWindowsHookEx(WH_MOUSE, _mouseHookProcedure, (IntPtr)0, AppDomain.GetCurrentThreadId());

                if (_hMouseHook == 0)
                {
                    var errorCode = Marshal.GetLastWin32Error();
                    Stop(true, false, false);
                    throw new Win32Exception(errorCode);
                }
            }

            // install Keyboard hook only if it is not installed and must be installed
            if (_hKeyboardHook == 0 && installKeyboardHook)
            {
                _keyboardHookProcedure = KeyboardHookProc;
                var hInstance = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
                _hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, _keyboardHookProcedure, hInstance, 0);

                if (_hKeyboardHook == 0)
                {
                    var errorCode = Marshal.GetLastWin32Error();
                    Stop(false, true, false);
                    throw new Win32Exception(errorCode);
                }
            }
        }

        public void Stop(bool uninstallMouseHook = true, bool uninstallKeyboardHook = true, bool throwExceptions = true)
        {
            if (_hMouseHook != 0 && uninstallMouseHook)
            {
                var retMouse = UnhookWindowsHookEx(_hMouseHook);
                _hMouseHook = 0;
                if (retMouse == 0 && throwExceptions)
                {
                    var errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode);
                }
            }

            if (_hKeyboardHook != 0 && uninstallKeyboardHook)
            {
                var retKeyboard = UnhookWindowsHookEx(_hKeyboardHook);
                _hKeyboardHook = 0;
                if (retKeyboard == 0 && throwExceptions)
                {
                    var errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode);
                }
            }
        }

        // ############################### Mouse Input ############################### //
        #region Mouse Input        

        private int MouseHookProc(int nCode, int wParam, IntPtr lParam) {

            if ((nCode >= 0) && (MouseDown != null || MouseUp != null || MouseMove != null || MouseHover != null || MouseWheel != null || MouseDoubleClick != null)) {

                var state = Mouse.GetState();
                var x = state.X;
                var y = state.Y;

                switch (wParam) {

                    //Scroll Wheel
                    case WM_MOUSEWHEEL:
                        if (MouseWheel != null) MouseWheel(null, new MouseEventArgs(MouseButton.None, 0, x, y, (wParam >> 16) / 120));
                        break;
                    // Mouse Movement			
                    case WM_MOUSEMOVE:
                        if (MouseMove != null) MouseMove(null, new MouseEventArgs(MouseButton.None, 0, x, y, 0));
                        break;
                    case WM_MOUSEHOVER:
                        if (MouseHover != null) MouseHover(null, new MouseEventArgs(MouseButton.None, 0, x, y, 0));
                        break;                    
                    //Left Mouse Button
                    case WM_LBUTTONDOWN:
                        if (MouseDown != null) MouseDown(null, new MouseEventArgs(MouseButton.Left, 1, x, y, 0));
                        break;
                    case WM_LBUTTONUP:
                        if (MouseUp != null) MouseUp(null, new MouseEventArgs(MouseButton.Left, 1, x, y, 0));
                        break;
                    case WM_LBUTTONDBLCLK:
                        if (MouseDoubleClick != null) MouseDoubleClick(null, new MouseEventArgs(MouseButton.Left, 2, x, y, 0));
                        break;
                    //Right Mouse Button
                    case WM_RBUTTONDOWN:
                        if (MouseDown != null) MouseDown(null, new MouseEventArgs(MouseButton.Right, 1, x, y, 0));
                        break;
                    case WM_RBUTTONUP:
                        if (MouseUp != null) MouseUp(null, new MouseEventArgs(MouseButton.Right, 1, x, y, 0));
                        break;
                    case WM_RBUTTONDBLCLK:
                        if (MouseDoubleClick != null) MouseDoubleClick(null, new MouseEventArgs(MouseButton.Right, 2, x, y, 0));
                        break;
                    //Middle Mouse Button
                    case WM_MBUTTONDOWN:
                        if (MouseDown != null) MouseDown(null, new MouseEventArgs(MouseButton.Middle, 1, x, y, 0));
                        break;
                    case WM_MBUTTONUP:
                        if (MouseUp != null) MouseUp(null, new MouseEventArgs(MouseButton.Middle, 1, x, y, 0));
                        break;
                    case WM_MBUTTONDBLCLK:
                        if (MouseDoubleClick != null) MouseDoubleClick(null, new MouseEventArgs(MouseButton.Middle, 2, x, y, 0));
                        break;
                    //Extra Buttons
                    case WM_XBUTTONDOWN:
                        if ((wParam & 0x10000) != 0) {
                            if (MouseDown != null) MouseDown(null, new MouseEventArgs(MouseButton.X1, 1, x, y, 0));
                        } else if ((wParam & 0x20000) != 0) {
                            if (MouseDown != null) MouseDown(null, new MouseEventArgs(MouseButton.X2, 1, x, y, 0));
                        }
                        break;
                    case WM_XBUTTONUP:
                        if ((wParam & 0x10000) != 0) {
                            if (MouseUp != null) MouseUp(null, new MouseEventArgs(MouseButton.X1, 1, x, y, 0));
                        } else if ((wParam & 0x20000) != 0) {
                            if (MouseUp != null) MouseUp(null, new MouseEventArgs(MouseButton.X2, 1, x, y, 0));
                        }
                        break;
                    case WM_XBUTTONDBLCLK:
                        if ((wParam & 0x10000) != 0) {
                            if (MouseDoubleClick != null) MouseDoubleClick(null, new MouseEventArgs(MouseButton.X1, 2, x, y, 0));
                        } else if ((wParam & 0x20000) != 0) {
                            if (MouseDoubleClick != null) MouseDoubleClick(null, new MouseEventArgs(MouseButton.X2, 2, x, y, 0));
                        }
                        break;
                }
            }

            return CallNextHookEx(_hMouseHook, nCode, wParam, lParam);
        }

        #endregion

        // ############################### Keyboard Input ############################### //
        #region Keyboard Input        

        [StructLayout(LayoutKind.Sequential)]
        private class KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        [DllImport("user32")]
        private static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetKeyState(int vKey);

        [DllImport("user32")]
        private static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);

        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam) {            

            if (_game.IsActive && nCode >= 0) {
                
                var myKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));

                switch (wParam) {
                    case WM_KEYDOWN:
                        if (KeyDown != null) KeyDown(null, new KeyEventArgs((Keys) wParam));

                        var isDownShift = ((GetKeyState(VK_SHIFT) & 0x80) == 0x80);
                        var isDownCapslock = (GetKeyState(VK_CAPITAL) != 0);

                        var keyState = new byte[256];
                        GetKeyboardState(keyState);
                        var inBuffer = new byte[2];
                        if (ToAscii(myKeyboardHookStruct.vkCode, myKeyboardHookStruct.scanCode, keyState, inBuffer, myKeyboardHookStruct.flags) == 1) {
                            var key = (char)inBuffer[0];
                            if ((isDownCapslock ^ isDownShift) && Char.IsLetter(key)) key = Char.ToUpper(key);
                            if (CharEntered != null) CharEntered(null, new CharacterEventArgs(key, lParam.ToInt32()));
                        }

                        break;
                    case WM_KEYUP:
                        if (KeyUp != null) KeyUp(null, new KeyEventArgs((Keys) wParam));
                        break;                    
                }
            }

            return CallNextHookEx(_hKeyboardHook, nCode, wParam, lParam);
        }

        #endregion
    }
}
