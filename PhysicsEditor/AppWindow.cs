using HipsterEngine.Core.Desktop;
using PhysicsEditor.Screens;

namespace PhysicsEditor
{
    public class AppWindow : GameWindowGPU
    {
        public AppWindow() : base(420, 780)
        {
            Engine.SetStartScreen(new StartScreen());
        }
    }
}