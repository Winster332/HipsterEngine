using HipsterEngine.Core.UI.Components;
using HipsterEngine.Core.UI.Components.Buttons;
using HipsterEngine.Core.UI.Components.Screens;
using HipsterEngine.UI;
using HipsterEngine.UI.Components;
using HipsterEngine.UI.Components.Layouts;
using SkiaSharp;

namespace SimpleUI.Core.Screens
{
    public class StartupScreen : Screen
    {
        public HipsterEngine.UI.UIController ui;
        
        public override void OnLoad()
        {
            Paint += OnPaint;
            ui = new UIController(HipsterEngine);
            ui.SetScreen(this);


            var layout = new LayoutAbsolute(HipsterEngine)
            {
                X = 200,
                Y = 200,
                Width = 200,
                Height = 200
            };

            var label = new Label(HipsterEngine)
            {
                X = 0,
                Y = 100,
                Text = "Hello HipsterEngine"
            };
            layout.AddView(label);
            ui.SetContentView(layout);
            AddView(new RectButton(Width / 2, Height / 2, 100, 25, "BUTTON"));
        }

        private void OnPaint(UIElement element, SKCanvas canvas)
        {
            canvas.Save();
            canvas.DrawRect(100, 100, 100, 100, new SKPaint
            {
                IsAntialias = true,
                Color = new SKColor(150, 150, 150)
            });
            canvas.Restore();
            
            ui.Draw();
        }
    }
}