using System.Windows.Forms;
using HipsterEditor.Physics.Screens;
using HipsterEngine.Core.Desktop;

namespace HipsterEditor.Physics
{
    public class MainWindow : GameWindowGPU
    {
        public MainWindow(int width, int height) : base(width, height)
        {
            Engine.SetStartScreen(new MainScreen());
        }
    }
}