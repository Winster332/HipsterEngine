using FlatPlant.Screens;
using HipsterEngine.Desktop;

namespace FlatPlant
{
    public class AppWindow : GameWindowGPU
    {
        public AppWindow() : base(1024, 720)
        {
            Engine.SetStartScreen(new GameScreen());
        }
    }
}