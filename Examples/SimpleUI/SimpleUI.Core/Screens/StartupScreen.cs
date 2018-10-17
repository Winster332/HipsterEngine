using HipsterEngine.Core.UI.Components.Buttons;
using HipsterEngine.Core.UI.Components.Screens;

namespace SimpleUI.Core.Screens
{
    public class StartupScreen : Screen
    {
        public override void OnLoad()
        {
            AddView(new RectButton(Width / 2, Height / 2, 100, 25, "BUTTON"));
        }
    }
}