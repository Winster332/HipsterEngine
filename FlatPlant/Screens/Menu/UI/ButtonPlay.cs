using System;
using ConsoleApplication2.UI.Components.Buttons;
using FlatPlant.Extensions;
using SkiaSharp;

namespace FlatPlant.Screens.UI
{
    public class ButtonPlay : CircleButton
    {
        public SKPaint Paint1 { get; set; }
        public SKPaint Paint2 { get; set; }
        public TimerWatch Timer { get; set; }
        
        public ButtonPlay(float x, float y, float radius, string text) : base(x, y, radius, text)
        {
            Paint1 = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                StrokeWidth = 7,
                Color = new SKColor(180, 100, 0, 150)
            };
            
            Paint2 = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                StrokeWidth = 7,
                Color = new SKColor(180, 100, 0, 150)
            };
            
            Timer = new TimerWatch();
            Timer.Tick += tick => { Radius += (float)Math.Cos(tick / 10.0f); };

            MouseUp += (element, state) =>
            {
                Timer.Start(1, 45);
            };
        }

        public override void Draw(SKCanvas canvas)
        {
            Timer.Update();
            
            canvas.DrawCircle(X, Y, Radius, Paint1);
            canvas.DrawCircle(X, Y, Radius-10, Paint1);
            canvas.DrawCircle(X, Y, Radius / 2, Paint2);
            
            var path = new SKPath();
            path.MoveTo(X - 30, Y - Radius / 2 + 40);
            path.LineTo(X - 30 + Radius / 2 - 10, Y);
            path.LineTo(X - 30, Y + Radius / 2 - 40);
            path.Close();
            
            canvas.DrawPath(path, new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = new SKColor(50, 50, 50)
            });
        }
    }
}