using System;
using System.Collections.Generic;
using Box2DX.Common;
using HoleVortex.Core.Models.Behaviors.Common;
using HoleVortex.Core.Models.Meshes;
using SkiaSharp;

namespace HoleVortex.Core.Background
{
    public class ParallaxBackground : Behavior
    {
        private HipsterEngine.Core.HipsterEngine _engine;
        private Triangle _triangle;
        public List<Vec3> Stars { get; set; }
        public SKPaint PaintStars { get; set; }
        private Random _random;
        private SKColor _clearColor;
        
        public ParallaxBackground(HipsterEngine.Core.HipsterEngine engine, Triangle triangle)
        {
            _engine = engine;
            _triangle = triangle;
            
            Update += OnUpdate;
            Paint += OnPaint;
            
            Stars = new List<Vec3>();
            PaintStars = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = new SKColor(250, 250, 250, 100)
            };
            
            _random = new Random();

            for (var i = 0; i < 50; i++)
            {
                Stars.Add(new Vec3(_random.Next(0, (int)_engine.Surface.Width), _random.Next(0, (int)_engine.Surface.Height*4), _random.Next(5, 10)));
            }
            
            _clearColor = new SKColor((byte)_random.Next(20, 50), (byte)_random.Next(20, 50), (byte)_random.Next(20, 50));
        }

        private void OnUpdate()
        {
        }

        private void OnPaint()
        {
            _engine.Surface.Canvas.Clear(_clearColor);
            
            for (var i = 0; i < Stars.Count; i++)
            {
                var star = Stars[i];
                var x = star.X;
                var y = star.Y;
                var radius = star.Z;
                
                _engine.Surface.Canvas.DrawCircle(x + _engine.Surface.Canvas.Camera.X / 0.8f, y + _engine.Surface.Canvas.Camera.Y / 0.8f, radius, PaintStars);
            }
        }
    }
}