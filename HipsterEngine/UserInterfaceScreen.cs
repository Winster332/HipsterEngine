using ConsoleApplication2.UI.Components.Buttons;
using ConsoleApplication2.UI.Components.Layout;
using ConsoleApplication2.UI.Components.Screens;

namespace ConsoleApplication2
{
    public class UserInterfaceScreen : Screen
    {
        public override void OnLoad()
        {
            var l = new LinearLayout(LinearLayoutOrientation.Horizontal);
            l.Width = Width;
            l.Height = 60;
            l.StepPosition = 10;
            l.AddElement(new RectButton(0, 0, 80, 50, "BUTTON"));
            l.AddElement(new RectButton(0, 0, 80, 50, "BUTTON"));
            l.AddElement(new RectButton(0, 0, 80, 50, "BUTTON"));
            
            AddView(l);
        }
    }
}