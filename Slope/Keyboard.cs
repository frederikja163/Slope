using System;
using System.Collections.Generic;
using OpenTK.Windowing.GraphicsLibraryFramework;
using GlfwWindow = OpenTK.Windowing.GraphicsLibraryFramework.Window;

namespace Slope
{
    public enum Key
    {
        Unknown = -1,
        Space = 32,
        Apostrophe = 39,
        Comma = 44,
        Minus = 45,
        Period = 46,
        Slash = 47,
        Key0 = 48,
        Key1 = 49,
        Key2 = 50,
        Key3 = 51,
        Key4 = 52,
        Key5 = 53,
        Key6 = 54,
        Key7 = 55,
        Key8 = 56,
        Key9 = 57,
        Semicolon = 59,
        Equal = 61,
        A = 65,
        B = 66,
        C = 67,
        D = 68,
        E = 69,
        F = 70,
        G = 71,
        H = 72,
        I = 73,
        J = 74,
        K = 75,
        L = 76,
        M = 77,
        N = 78,
        O = 79,
        P = 80,
        Q = 81,
        R = 82,
        S = 83,
        T = 84,
        U = 85,
        V = 86,
        W = 87,
        X = 88,
        Y = 89,
        Z = 90,
        LeftBracket = 91, 
        BackSlash = 92, 
        RightBracket = 93, 
        Grave = 96, 
        World1 = 161,
        World2 = 162,
        Escape = 256,
        Enter = 257,
        Tab = 258,
        Backspace = 259,
        Insert = 260,
        Delete = 261,
        Right = 262,
        Left = 263,
        Up = 264,
        Down = 265,
        PageUp = 266,
        PageDown = 267,
        Home = 268,
        End = 269,
        CapsLock = 280,
        ScrollLock = 281,
        NumLock = 282,
        PrintScreen = 283,
        Pause = 284,
        F1 = 290,
        F2 = 291,
        F3 = 292,
        F4 = 293,
        F5 = 294,
        F6 = 295,
        F7 = 296,
        F8 = 297,
        F9 = 298,
        F10 = 299,
        F11 = 300,
        F12 = 301,
        F13 = 302,
        F14 = 303,
        F15 = 304,
        F16 = 305,
        F17 = 306,
        F18 = 307,
        F19 = 308,
        F20 = 309,
        F21 = 310,
        F22 = 311,
        F23 = 312,
        F24 = 313,
        F25 = 314,
        Keypad0 = 320,
        Keypad1 = 321,
        Keypad2 = 322,
        Keypad3 = 323,
        Keypad4 = 324,
        Keypad5 = 325,
        Keypad6 = 326,
        Keypad7 = 327,
        Keypad8 = 328,
        Keypad9 = 329,
        KeypadDecimal = 330,
        KeypadDivide = 331,
        KeypadMultiply = 332,
        KeypadSubtract = 333,
        KeypadAdd = 334,
        KeypadEnter = 335,
        KeypadEqual = 336,
        LeftShift = 340,
        LeftControl = 341,
        LeftAlt = 342,
        LeftSuper = 343,
        RightShift = 344,
        RightControl = 345,
        RightAlt = 346,
        RightSuper = 347,
        Men = 348
    }

    [Flags]
    public enum KeyState : byte
    {
        Released = 1,
        Pressed = 2,
        JustPressed = Pressed | 4,
        JustReleased = Released | 8
    }

    public sealed unsafe class Keyboard
    {
        public struct KeyEventArgs
        {
            public Key Key { get; }
            public int KeyCode { get; }
            public KeyState State { get; }

            internal KeyEventArgs(Key key, int keyCode, KeyState state)
            {
                Key = key;
                KeyCode = keyCode;
                State = state;
            }
        }

        private readonly Dictionary<Key, KeyState> _keyStates;
        private readonly List<(Key key, int code)> _pressedKeys;

        internal Keyboard(GlfwWindow* handle)
        {
            _keyStates = new Dictionary<Key, KeyState>();
            _pressedKeys = new List<(Key key, int code)>();
            
            GLFW.SetKeyCallback(handle, (window, keyRaw, code, action, mods) =>
            {
                var key = (Key) keyRaw;
                
                KeyState state;
                if (action == InputAction.Press && (this[key] & KeyState.Released) == KeyState.Released)
                {
                    state = KeyState.JustPressed;
                    _pressedKeys.Add((key, code));
                }
                else if (action == InputAction.Release)
                {
                    state = KeyState.JustReleased;
                }
                else
                {
                    return;
                }
                
                _keyStates[key] = state;
            });
        }

        internal void PrePoll()
        {
            for (int i = _pressedKeys.Count - 1; i >= 0; i--)
            {
                var pressedKey = _pressedKeys[i];
                var key = pressedKey.key;
                switch (_keyStates[key])
                {
                    case KeyState.JustPressed:
                        _keyStates[key] = KeyState.Pressed;
                        break;
                    case KeyState.JustReleased:
                        _keyStates[key] = KeyState.Released;
                        _pressedKeys.RemoveAt(i);
                        break;
                    default:
                        break;
                }
            }
        }
        
        internal void PostPoll()
        {
            for (int i = _pressedKeys.Count - 1; i >= 0; i--)
            {
                var pressedKey = _pressedKeys[i];
                var key = pressedKey.key;
                var code = pressedKey.code;
                var state = _keyStates[key];
            
                var args = new KeyEventArgs(key, code, state);
                
                var delegates = OnKey?.GetInvocationList();
                if (delegates == null) return;
                foreach (var d in delegates)
                {
                    var e = (KeyEvent) d;
                    if (e.Invoke(this, args))
                    {
                        return;
                    }
                }
            }
        }

        public bool IsPressed(Key key)
        {
            return (this[key] & KeyState.Pressed) == KeyState.Pressed;
        }

        public KeyState this[Key key] => _keyStates.TryGetValue(key, out var state) ? state : KeyState.Released;
        
        public delegate bool KeyEvent(Keyboard sender, KeyEventArgs e);

        public KeyEvent? OnKey;
    }
}