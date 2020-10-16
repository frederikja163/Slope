using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Slope
{
    public sealed class Window : GameWindow
    {
        public Window()
            : base(new GameWindowSettings(){IsMultiThreaded = true}, new NativeWindowSettings(){Title =  "Slope"})
        {
            RenderThreadStarted += OnRenderThreadStarted;
            RenderFrame += OnRenderFrame;
            Resize += OnResize;
            Closed += OnClose;
        }

        private void OnRenderThreadStarted()
        {
            GL.LoadBindings(new GLFWBindingsContext());
            GL.ClearColor(Color4.Magenta);
        }

        private void OnRenderFrame(FrameEventArgs obj)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            SwapBuffers();
        }

        private void OnClose()
        {
        }

        private void OnResize(ResizeEventArgs obj)
        {
            GL.Viewport(0, 0, obj.Width, obj.Height);
        }
    }
}