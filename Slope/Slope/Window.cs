using OpenTK.Windowing.Desktop;

namespace Slope
{
    public class Window : GameWindow
    {
        public Window()
            : base(new GameWindowSettings(){IsMultiThreaded = true}, new NativeWindowSettings(){Title =  "Slope"})
        {
        }
    }
}