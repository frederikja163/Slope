using System;
using OpenTK.Windowing.GraphicsLibraryFramework;
using GlfwWindow = OpenTK.Windowing.GraphicsLibraryFramework.Window;

namespace Slope
{
    public sealed unsafe class Window : IDisposable
    {
        private readonly GlfwWindow* _window;

        public Window(int width, int height, string title)
        {
            _window = GLFW.CreateWindow(width, height, title, null, null);
        }

        public bool IsRunning => !GLFW.WindowShouldClose(_window);

        public void MakeCurrent()
        {
            GLFW.MakeContextCurrent(_window);
        }

        public void SwapBuffers()
        {
            GLFW.SwapBuffers(_window);
        }

        public void Dispose()
        {
            GLFW.DestroyWindow(_window);
        }

        public static void PollEvents()
        {
            GLFW.PollEvents();
        }
        
        public static void InitGlfw()
        {
            if (!GLFW.Init())
            {
                throw new Exception("Glfw failed to initialize!");
            }
        }

        public static void TerminateGlfw()
        {
            GLFW.Terminate();
        }
    }
}