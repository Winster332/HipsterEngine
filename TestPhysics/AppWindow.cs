using HipsterEngine.Core.Desktop;

namespace TestPhysics
{
    public class AppWindow : GameWindowGPU
    {
        public AppWindow() : base(420, 780)
        {
            Engine.SetStartScreen(new MainScreen());
        }
    }
}