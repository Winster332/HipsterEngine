using HipsterEngine.Core.Desktop;
using SimplePhysics.Screens;

namespace SimplePhysics
{
    public class AppWindow : GameWindowGPU
    {
        public AppWindow() : base(1024, 720)
        {
            Engine.SetStartScreen(new GameScreen());
        }
    }
}