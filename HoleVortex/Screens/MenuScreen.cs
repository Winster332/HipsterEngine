using System;
using System.IO;
using System.Linq;
using ConsoleApplication2.UI.Components;
using ConsoleApplication2.UI.Components.Screens;
using ConsoleApplication2.UI.Events;
using HoleVortex.Asserts;
using HoleVortex.Models;
using HoleVortex.Screens.UI;
using SkiaSharp;
using TestOpenTK;
using TestOpenTK.Extensions;

namespace HoleVortex.Screens
{
    public class MenuScreen : Screen
    {
        public LayoutRecord LayoutRecords { get; set; }
        public Label LabelTouchMe { get; set; }
        public PlanetStart Planet { get; set; }
        public Triangle Triangle { get; set; }
        public SKColor BackgroundColor { get; set; }
        private float _time;
        
        public override void OnLoad()
        {
            Update += OnUpdate;
            Paint += OnPaint;
            
            LayoutRecords = new LayoutRecord(HipsterEngine);
            Planet = new PlanetStart(HipsterEngine, Width / 2, Height / 4, Height / 8);
            var triangleRadius = Planet.Radius / 4;
            var y = Planet.Y - Planet.Radius - triangleRadius + 2;
            Triangle = new Triangle(HipsterEngine, Width / 2, y, triangleRadius);
            LabelTouchMe = new Label(HipsterEngine, Width / 2, Height / 2+10, "TOUCH SCREEN", new SKPaint
            {
                Color = new SKColor(150, 150, 150, 100),
                IsAntialias = true,
                TextAlign = SKTextAlign.Center,
                TextSize = 20
            }, 1);
            BackgroundColor = new SKColor(50, 50, 50);
            _time = 0;
            
            MouseDown += OnMouseDown;


            paint = new SKPaint
            {
                IsAntialias = true,
                Color = new SKColor(0, 0, 0)
            };
        }

        public SKBitmap bitmap;
        public SKPaint paint;

        private void OnMouseDown(UIElement element, MouseState mousestate)
        {
            HipsterEngine.Screens.SetScreen(new GameScreen());
        }

        private void OnUpdate(double time, float dt)
        {
            _time += 0.03f;
            LabelTouchMe.Scale = (float) Math.Cos(Math.Sin(_time) / 2);
        }

        private void OnPaint(UIElement element, SKCanvas canvas)
        {
            canvas.Clear(BackgroundColor);
            
            LayoutRecords.Draw();
            Planet.Draw();
            Triangle.Draw();
            
            HipsterEngine.Surface.Canvas.Save();
            HipsterEngine.Surface.Canvas.Translate(Width / 2, Height / 2);
          //  HipsterEngine.Surface.Canvas.RotateRadians(_time);
        //    HipsterEngine.Surface.Canvas.Scale(0.5f, 0.5f);
          //  if (bitmap != null)
         //       HipsterEngine.Surface.Canvas.DrawBitmap(bitmap, -bitmap.Width / 2,-bitmap.Height / 2, paint);
                //HipsterEngine.Surface.Canvas.DrawBitmap(bitmap, SKRect.Create(0, 0, 100, 100), paint);
            HipsterEngine.Surface.Canvas.Restore();
            
            LabelTouchMe.Draw();
        }
    }
}