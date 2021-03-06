﻿using System;
using HipsterEngine.Core.Graphics;
using HoleVortex.Core.IO;
using SkiaSharp;

namespace HoleVortex.Core.Screens.UI
{
    public class LayoutRecord : IDisposable
    {
        public float Y { get; set; }
        public float Radius { get; set; }
        public string TextRecord { get; set; }
        public SKPaint Paint { get; set; }
        public SKPaint PaintText { get; set; }
        private HipsterEngine.Core.HipsterEngine _engine;
        private float _x1Line1;
        private float _y1Line1;
        private float _x2Line1;
        private float _y2Line1;
        private float _x1Line2;
        private float _y1Line2;
        private float _x2Line2;
        private float _y2Line2;
        private float _circleX;
        private float _circleY;
        private float _textX;
        private float _textY;
        private Canvas canvas;
        
        public LayoutRecord(HipsterEngine.Core.HipsterEngine engine)
        {
            _engine = engine;
            canvas = _engine.Surface.Canvas;
            
            Radius = _engine.Surface.Width / 5;
            Y = _engine.Surface.Height - _engine.Surface.Height / 4;
            _x1Line1 = 0;
            _y1Line1 = Y;
            _x2Line1 = _engine.Surface.Width / 2 - Radius;
            _y2Line1 = Y;
            _x1Line2 = _engine.Surface.Width / 2 + Radius;
            _y1Line2 = Y;
            _x2Line2 = _engine.Surface.Width;
            _y2Line2 = Y;
            _circleX = _engine.Surface.Width / 2;
            _circleY = Y;
            _textX = _engine.Surface.Width / 2 - 1;
            _textY = Y + 16;
            TextRecord = "0";
            
            Paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2,
                IsAntialias = true,
                PathEffect = SKPathEffect.CreateDash(new[] {10.0f, 10.0f}, 10),
                Color = new SKColor(150, 150, 150)
            };
            
            PaintText = new SKPaint
            {
                IsAntialias = true,
                Color = new SKColor(150, 150, 150),
                TextAlign = SKTextAlign.Center,
                Typeface = Assets.Typeface,
                TextSize = 50
            };
        }

        public void Draw()
        {
            canvas.DrawLine(_x1Line1, _y1Line1, _x2Line1, _y2Line1, Paint);
            canvas.DrawLine(_x1Line2, _y1Line2, _x2Line2, _y2Line2, Paint);
            canvas.DrawCircle(_circleX, _circleY, Radius, Paint);
            canvas.DrawText(TextRecord, _textX, _textY, PaintText);
        }

        public void Dispose()
        {
            Paint?.Dispose();
            PaintText?.Dispose();
        }
    }
}