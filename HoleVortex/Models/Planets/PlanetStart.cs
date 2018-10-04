﻿using System;
using System.Collections.Generic;
using Box2DX.Common;
using SkiaSharp;

namespace HoleVortex.Models
{
    public class PlanetStart : Planet, IDisposable
    {
        public SKPaint PaintText { get; set; }
        public SKPaint Paint1 { get; set; }
        public SKPaint Paint2 { get; set; }
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
            
            Paint1 = new SKPaint
            {
                IsAntialias = true,
                Color = new SKColor(150, 150, 150),
                StrokeWidth = 3,
                Style = SKPaintStyle.Stroke
            };
            Paint2 = new SKPaint
            {
                IsAntialias = true,
                Color = new SKColor(150, 150, 150),
                PathEffect = SKPathEffect.CreateDash(new[] {20.0f, 20.0f}, 10),
                StrokeWidth = 10,
                Style = SKPaintStyle.Stroke
            };
        }

        public void Draw()
        {
            _engine.Surface.Canvas.Save();
            _engine.Surface.Canvas.Translate(X, Y);
            _engine.Surface.Canvas.RotateRadians(Angle);
            _engine.Surface.Canvas.DrawCircle(0, 0, Radius, Paint1);
            _engine.Surface.Canvas.DrawCircle(0, 0, Radius - 5, Paint2);
            _engine.Surface.Canvas.DrawCircle(0, 0, Radius - 5, Paint1);
            _engine.Surface.Canvas.Restore();
            _engine.Surface.Canvas.DrawText(Text, X, Y + 26, PaintText);
        }

        public void Dispose()
        {
            PaintText?.Dispose();
            Paint1.Dispose();
            Paint2.Dispose();
        }
    }
}