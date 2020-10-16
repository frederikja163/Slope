using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using GlfwWindow = OpenTK.Windowing.GraphicsLibraryFramework.Window;

namespace Slope
{
    public enum MouseButton
    {
        Left = 0,
        Right = 1,
        Middle = 3,
        Button1 = 4,
        Button2 = 5,
        Button3 = 6,
        Button4 = 7,
    }
    
    public enum MouseButtonState
    {
        Released = 1,
        Pressed = 2,
        JustPressed = Pressed | 4,
        JustReleased = Released | 8
    }

    public unsafe sealed class Mouse
    {
        public struct MouseButtonEventArgs
        {
            public MouseButton Button { get; }
            public MouseButtonState State { get; }
            public Vector2 MousePosition { get; }

            internal MouseButtonEventArgs(MouseButton button, MouseButtonState state, Vector2 mousePosition)
            {
                Button = button;
                State = state;
                MousePosition = mousePosition;
            }
        }

        private readonly Dictionary<MouseButton, MouseButtonState> _buttonStates;
        private List<MouseButton> _pressedButtons;
        
        internal Mouse(GlfwWindow* handle)
        {
            GLFW.GetCursorPos(handle, out var x, out var y);
            Position = new Vector2((float)x, (float)y);
            
            _buttonStates = new Dictionary<MouseButton, MouseButtonState>();
            _pressedButtons = new List<MouseButton>();
            
            GLFW.SetMouseButtonCallback(handle, (window, buttonRaw, actionRaw, mods) =>
            {
                var button = (MouseButton) buttonRaw;

                MouseButtonState state;
                if (actionRaw == InputAction.Press)
                {
                    state = MouseButtonState.JustPressed;
                    _pressedButtons.Add(button);
                }
                else if (actionRaw == InputAction.Release)
                {
                    state = MouseButtonState.JustReleased;
                }
                else
                {
                    return;
                }

                _buttonStates[button] = state;
            });

            GLFW.SetCursorPosCallback(handle, (window, x, y) =>
            {
                Position = new Vector2((float)x, (float)y);
                var delegates = OnMouseMoved?.GetInvocationList();
                if (delegates == null) return;
                foreach (var d in delegates)
                {
                    var e = (MouseMoveEvent) d;
                    if (e.Invoke(this, Position))
                    {
                        return;
                    }
                }
            });
        }

        internal void PrePoll()
        {
            for (int i = _pressedButtons.Count - 1; i >= 0; i--)
            {
                var button = _pressedButtons[i];
                switch (_buttonStates[button])
                {
                    case MouseButtonState.JustPressed:
                        _buttonStates[button] = MouseButtonState.Pressed;
                        break;
                    case MouseButtonState.JustReleased:
                        _buttonStates[button] = MouseButtonState.Released;
                        _pressedButtons.RemoveAt(i);
                        break;
                    default:
                        break;
                }
            }
        }
        
        internal void PostPoll()
        {
            for (int i = _pressedButtons.Count - 1; i >= 0; i--)
            {
                var button = _pressedButtons[i];
                var state = _buttonStates[button];
                var args = new MouseButtonEventArgs(button, state, Position);
                
                _buttonStates[button] = state;
                
                var delegates = OnMouseButton?.GetInvocationList();
                if (delegates == null) return;
                foreach (var d in delegates)
                {
                    var e = (MouseButtonEvent) d;
                    if (e.Invoke(this, args))
                    {
                        return;
                    }
                }
            }
        }

        public MouseButtonState this[MouseButton b] =>
            _buttonStates.TryGetValue(b, out var s) ? s : MouseButtonState.Released; 

        public delegate bool MouseButtonEvent(Mouse sender, MouseButtonEventArgs e);
        public delegate bool MouseMoveEvent(Mouse sender, Vector2 position);
        
        public MouseMoveEvent? OnMouseMoved;
        public MouseButtonEvent? OnMouseButton;
        
        public Vector2 Position { get; private set; }
    }
}