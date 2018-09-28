using Box2DX.Common;
using ConsoleApplication2.UI.Components;
using ConsoleApplication2.UI.Components.Screens;
using ConsoleApplication2.UI.Events;
using SkiaSharp;
using Math = System.Math;

namespace HoleVortex.Screens
{
    public class TestScreen : Screen
    {
        public override void OnLoad()
        {
            Paint += OnPaint;
        }
        
        private void OnPaint(UIElement element, SKCanvas canvas)
        {
            var path = new SKPath();
            path.MoveTo(0, 0);
            path.CubicTo(0, 0, 50, 50, 100, 0);
            
            HipsterEngine.Surface.Canvas.Save();
            HipsterEngine.Surface.Canvas.Translate(Width / 2, Height / 2);
            HipsterEngine.Surface.Canvas.DrawPath(path, new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                Color = new SKColor(226, 88, 34)
            });
            HipsterEngine.Surface.Canvas.Restore();
        }
    }
}