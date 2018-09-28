using HipsterEngine.Desktop;
using HoleVortex.Screens;

namespace HoleVortex
{
    public class AppWindow : GameWindowGPU
    {
        public AppWindow() : base(420, 780)
        {
            Engine.SetStartScreen(new MenuScreen());
        }
    }
}