using System.Linq;
using HipsterEngine.Core.UI.Components;
using HipsterEngine.Core.UI.Components.Screens;
using SkiaSharp;

namespace CrossZero.Core.Screens
{
    public class StartScreen : Screen
    {
        public override void OnLoad()
        {
            Paint += OnPaint;
        }

        private void OnPaint(UIElement element, SKCanvas canvas)
        {
            HipsterEngine.Surface.Canvas.DrawBitmap(Assets.Assets.Bitmaps.First(), new SKRect(0, 0, HipsterEngine.Surface.Width, HipsterEngine.Surface.Height), null);
        }
    }
}