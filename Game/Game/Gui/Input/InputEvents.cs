using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Ruminate.GUI.Framework {

    public enum MouseButton { None, Left, Right, Middle, X1, X2 }

    #region EventArgs 

    public delegate void CharEnteredHandler(object sender, CharacterEventArgs e);
    public delegate void KeyEventHandler(object sender, KeyEventArgs e);
    public delegate void MouseEventHandler(object sender, MouseEventArgs e);

    public class CharacterEventArgs : EventArgs {

        private readonly char _character;
        private readonly int _lParam;

        public CharacterEventArgs(char character, int lParam) {
            _character = character;
            _lParam = lParam;
        }

        public char Character { get { return _character; } }
        public int Param { get { return _lParam; } }
        public int RepeatCount { get { return _lParam & 0xffff; } }
        public bool ExtendedKey { get { return (_lParam & (1 << 24)) > 0; } }
        public bool AltPressed { get { return (_lParam & (1 << 29)) > 0; } }
        public bool PreviousState { get { return (_lParam & (1 << 30)) > 0; } }
        public bool TransitionState { get { return (_lParam & (1 << 31)) > 0; } }

    }

    public class KeyEventArgs : EventArgs {

        private Keys keyCode;

        public KeyEventArgs(Keys keyCode) {
            this.keyCode = keyCode;
        }

        public Keys KeyCode { get { return keyCode; } }
    }

    public class MouseEventArgs : EventArgs {

        public MouseButton Button { get; private set; }
        public int Clicks { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }        
        public int Delta { get; private set; }

        public Point Location { get { return new Point(X, Y); } }

        public MouseEventArgs(MouseButton button, int clicks, int x, int y, int delta) {
            Button = button;
            Clicks = clicks;
            X = x;
            Y = y;
            Delta = delta;
        }    
    }

    #endregion
}
