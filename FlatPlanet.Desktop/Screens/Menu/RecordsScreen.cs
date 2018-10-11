using HipsterEngine.Core.UI.Components;
using HipsterEngine.Core.UI.Components.Screens;
using SkiaSharp;

namespace FlatPlant.Screens
{
    public class RecordsScreen : SwypeScreen
    {
        public override void OnLoad()
        {
            Paint += OnPaint;
        }

        private void OnPaint(UIElement element, SKCanvas canvas)
        {
            var p = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                PathEffect = SKPathEffect.CreateDash(new[] { 10.0f, 10.0f }, 10),
                Color = new SKColor(210, 170, 0, 100)
            };
            
            canvas.DrawRect(40, 40, Width - 80, Height - 80, p);
            
            canvas.DrawText($"Current screen: {GetType().Name} [FPS: {HipsterEngine.DeltaTime.GetFPS()}]", 50, 75,
                new SKPaint
                {
                    TextSize = 20,
                    IsAntialias = true,
                    Color = new SKColor(100, 100, 100)
                });

            canvas.DrawText($"Life: {Enabled}", 50, 100, new SKPaint
            {
                TextSize = 20,
                    IsAntialias = true,
                Color = new SKColor(100, 100, 100)
            });

            HipsterEngine.Surface.Canvas.DrawCircle(Width / 2, Height / 2, 50, new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = new SKColor(100, 100, 100)
            });
        }
    }
}