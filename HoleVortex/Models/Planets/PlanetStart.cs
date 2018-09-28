using System;
using System.Collections.Generic;
using Box2DX.Common;
using SkiaSharp;

namespace HoleVortex.Models
{
    public class PlanetStart : Planet
    {
        public SKPaint PaintText { get; set; }
        public string Text { get; set; }

        public PlanetStart(ConsoleApplication2.HipsterEngine engine, float x, float y, float radius) : base(engine, x, y, radius, -1)
        {
            PaintText = new SKPaint
            {
                IsAntialias = true,
                Color = new SKColor(150, 150, 150),
                TextAlign = SKTextAlign.Center,
                TextSize = 80
            };
            Text = "1";
        }

        public void Draw()
        {
            var paint1 = new SKPaint
            {
                IsAntialias = true,
                Color = new SKColor(150, 150, 150),
                StrokeWidth = 3,
                Style = SKPaintStyle.Stroke
            };
            var paint = new SKPaint
            {
                IsAntialias = true,
                Color = new SKColor(150, 150, 150),
                PathEffect = SKPathEffect.CreateDash(new[] {20.0f, 20.0f}, 10),
                StrokeWidth = 10,
                Style = SKPaintStyle.Stroke
            };
            _engine.Surface.Canvas.Save();
            _engine.Surface.Canvas.Translate(X, Y);
            _engine.Surface.Canvas.RotateRadians(Angle);
            _engine.Surface.Canvas.DrawCircle(0, 0, Radius, paint1);
            _engine.Surface.Canvas.DrawCircle(0, 0, Radius - 5, paint);
            _engine.Surface.Canvas.DrawCircle(0, 0, Radius - 5, paint1);
            _engine.Surface.Canvas.Restore();
            _engine.Surface.Canvas.DrawText(Text, X, Y + 26, PaintText);
        }
    }
}