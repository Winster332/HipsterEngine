using System.Collections.Generic;
using ConsoleApplication2;
using ConsoleApplication2.Physics.Bodies;
using ConsoleApplication2.UI.Components;
using ConsoleApplication2.UI.Components.Screens;
using FlatPlant.Extensions;
using SkiaSharp;

namespace FlatPlant.Screens
{
    public class StartScreen : Screen
    {
        public SKPaint PaintWhite { get; set; }
        public SKPaint PaintStroke { get; set; }
        public TimerWatch Timer { get; set; }
        public TimerWatch TimerForIntent { get; set; }
        public int Number { get; set; }
        public byte Alpha = 0;
        
        public override void OnLoad()
        {
            PaintWhite = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2,
                IsAntialias = true,
                Color = new SKColor(150, 150, 150)
            };
            PaintStroke = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                PathEffect = SKPathEffect.CreateDash(new[] {10.0f, 10.0f}, 10),
                Color = new SKColor(210, 170, 0, 100)
            };
            Number = 3;
            
            Paint += OnPaint;
            Update += OnUpdate;
            
            TimerForIntent = new TimerWatch();
            TimerForIntent.Tick += tick => { Alpha = (byte) tick; };
            TimerForIntent.Complated += tick => HipsterEngine.Screens.SetScreen(new NavigatorScreen());
            
            Timer = new TimerWatch();
            Timer.Tick += tick =>
            {
                Number = 3 - tick;
            };
            Timer.Complated += tick =>
            {
                TimerForIntent.Start(0, 255);
            };
            Timer.Start(100, 3);
        }

        private void OnUpdate(double time, float dt)
        {
            TimerForIntent.Update();
            Timer.Update();
        }

        private void OnPaint(UIElement element, SKCanvas canvas)
        {
            canvas.DrawRect(0, 0, Width, Height, new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = new SKColor(30, 30, 30)
            });
            
            //canvas.DrawCircle(Width / 2, Height / 2, Width / 10, PaintWhite);
            canvas.DrawCircle(Width / 2, Height / 2, Width / 10, PaintStroke);
            canvas.DrawLine(0, Height / 2, Width / 2 - (Width / 10), Height / 2, PaintStroke);
            canvas.DrawLine(Width / 2, 0, Width / 2, Height / 2 - (Width / 10), PaintStroke);
            canvas.DrawLine(Width / 2, Height, Width / 2, Height / 2 + (Width / 10), PaintStroke);
            canvas.DrawLine(Width / 2 + (Width / 10), Height / 2, Width, Height / 2, PaintStroke);
            canvas.DrawCircle(Width / 2, Height / 2, Width / 15, new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = new SKColor(25, 25, 25)
            });
            canvas.DrawText(Number.ToString(), Width / 2, Height / 2 + 27,
                new SKPaint
                {
                    TextSize = 85,
                    IsAntialias = true,
                    TextAlign = SKTextAlign.Center,
                    Color = new SKColor(100, 100, 100)
                });
            canvas.DrawCircle(Width / 2, Height / 2, Width / 15, PaintStroke);
            
            canvas.DrawRect(0, 0, Width, Height, new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = new SKColor(0, 0, 0, Alpha)
            });
        }
    }
}