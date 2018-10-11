using HipsterEngine.Core.Desktop;
using HoleVortex.Desktop.Screens;

namespace HoleVortex.Desktop
{
    public class AppWindow : GameWindowGPU
    {
        public AppWindow() : base(420, 780)
        {
            Engine.SetStartScreen(new MenuScreen());
        }
    }
}