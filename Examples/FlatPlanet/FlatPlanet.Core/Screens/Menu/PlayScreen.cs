using System;
using FlatPlant.Screens.UI;
using HipsterEngine.Core.UI.Components;
using HipsterEngine.Core.UI.Components.Screens;
using SkiaSharp;

namespace FlatPlant.Screens
{
    public class PlayScreen : SwypeScreen
    {
        public ButtonPlay ButtonPlay { get; set; }
        
        public override void OnLoad()
        {
            Paint += OnPaint;
            
            ButtonPlay = new ButtonPlay(Width / 2, Height / 2, Width / 4, "");
            ButtonPlay.Click += (element, state) =>
            {
                Console.WriteLine();
            };

            MouseDown += (element, state) =>
            {
                ButtonPlay.OnMouseAction(state);
            };
            MouseUp += (element, state) => { ButtonPlay.OnMouseAction(state); };
            MouseMove += (element, state) => { ButtonPlay.OnMouseAction(state); };
        }

        private void OnPaint(UIElement element, SKCanvas canvas)
        {
            var p = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                PathEffect = SKPathEffect.CreateDash(new[] {10.0f, 10.0f}, 10),
                Color = new SKColor(210, 170, 0, 100)
            };
            var pp = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = new SKColor(50, 50, 50)
            };

            canvas.DrawRect(40, 40, Width - 80, Height - 80, p);
            canvas.DrawCircle(Width - 120, Y + 40, 36, p);
            canvas.DrawCircle(Width - 120, Y + 40, 36, pp);

            canvas.DrawText($"120", Width - 121, Y + 48,
                new SKPaint
                {
                    TextSize = 25,
                    IsAntialias = true,
                    TextAlign = SKTextAlign.Center,
                    Color = new SKColor(100, 100, 100)
                });

            //    canvas.DrawText($"Current screen: {GetType().Name} [FPS: {HipsterEngine.DeltaTime.GetFPS()}]", 20, 65,
            //        new SKPaint
            //         {
            //             TextSize = 20,
            //             IsAntialias = true,
            //            Color = new SKColor(100, 100, 100)
            //        });

            //    canvas.DrawText($"Life: {Enabled}", 20, 40, new SKPaint
            //    {
            //        TextSize = 20,
            //            IsAntialias = true,
            //        Color = new SKColor(100, 100, 100)
            //    });

            ButtonPlay.Draw(canvas);
        }
    }
}