using System;
using Box2DX.Common;
using SkiaSharp;

namespace ConsoleApplication2.UI.Components.GamePad
{
    public class AttackControl
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Radius { get; set; }
        public SKPaint PaintFill { get; set; }
        public SKPaint PaintBorder { get; set; }
        public event EventHandler Click; 
        private HipsterEngine _engine;
        
        public AttackControl(HipsterEngine engine, float radius)
        {
            _engine = engine;
            Radius = radius;
            X = Radius + Radius / 2;
            Y = _engine.Surface.Height - Radius - Radius / 2;

            PaintFill = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = new SKColor(100, 100, 100, 100)
            };
            
            PaintBorder = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                StrokeWidth = 5,
                Color = new SKColor(30, 30, 30)
            };

            _engine.Screens.CurrentScreen.MouseDown += (element, state) =>
            {
                if (IsIntersect(state.X, state.Y))
                {
                    PaintFill.Color = new SKColor(40, 40, 40);
                    Click?.Invoke(null, null);
                }
            };
            _engine.Screens.CurrentScreen.MouseUp += (element, state) =>
            {
                if (IsIntersect(state.X, state.Y))
                {
                    PaintFill.Color = new SKColor(100, 100, 100, 100);
                }
            };
        }
        
        public bool IsIntersect(float x, float y)
        {
            var distance = Vec2.Distance(new Vec2(X, Y), new Vec2(x, y));

            if (distance <= Radius)
                return true;
            else return false;
        }

        public void Draw()
        {
            _engine.Surface.Canvas.DrawCircle(X, Y, Radius, PaintFill);
            _engine.Surface.Canvas.DrawCircle(X, Y, Radius, PaintBorder);
        }
    }
}