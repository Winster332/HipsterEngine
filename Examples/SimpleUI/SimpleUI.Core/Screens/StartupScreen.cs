using System.Text;
using HipsterEngine.Core.UI.Components;
using HipsterEngine.Core.UI.Components.Buttons;
using HipsterEngine.Core.UI.Components.Screens;
using SkiaSharp;

namespace SimpleUI.Core.Screens
{
    public class StartupScreen : Screen
    {
        public override void OnLoad()
        {
            AddView(new RectButton(Width / 2, Height / 2, 100, 25, "BUTTON"));
            this.Paint += OnPaint;
        }

        private void OnPaint(UIElement element, SKCanvas canvas)
        {
           // var text = Encoding.Unicode.GetBytes("Hello world");
            canvas.DrawText("Hello world", 200, 200, new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Typeface = SKTypeface.FromFamilyName(
                    "Arial", 
                    SKFontStyleWeight.Bold, 
                    SKFontStyleWidth.Normal, 
                    SKFontStyleSlant.Upright),
                TextSize = 40,
                Color = new SKColor(200, 200, 200),
                IsStroke = false,
                IsAntialias = true
            });
        }
    }
}