using System;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Slope.Layers;

namespace Slope
{
    public sealed class Application : IDisposable
    {
        private readonly Window _window;
        private readonly LayerStack _layerStack;
        
        public Application()
        {
            Window.InitGlfw();
            _window = new Window(800, 600, "Slope");
            _window.MakeCurrent();
            
            GL.LoadBindings(new GLFWBindingsContext());
            
            GL.ClearColor(0, 1, 1, 1);
            GL.Enable(EnableCap.DepthTest);
            
            _layerStack = new LayerStack(new Game());
        }

        public void Run()
        {
            var stopWatch = new Stopwatch();
            float freq = (float)Stopwatch.Frequency;
            
            while (_window.IsRunning)
            {
                var dt = stopWatch.ElapsedTicks / freq;
                stopWatch.Restart();
                _layerStack.Update(dt);
                    
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                _layerStack.Draw();
                
                _window.SwapBuffers();
                
                Window.PollEvents();
            }
        }

        public void Dispose()
        {
            Window.TerminateGlfw();
        }
    }
}