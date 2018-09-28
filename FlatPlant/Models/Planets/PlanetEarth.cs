using System;
using ConsoleApplication2.Physics.Bodies;
using SkiaSharp;

namespace FlatPlant.Models.Planets
{
    public class PlanetEarth : Model, IPlanet
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Radius { get; set; }
        public float TimeAtmosfereRadius { get; set; } = 0;
        public SKPaint PaintPlanet { get; set; }
        public SKPaint PaintEarth { get; set; }
        public SKPaint PaintCore { get; set; }
        public SKPaint PaintCoreBorder { get; set; }
        public RigidBody RigidBody { get; set; }
        
        public PlanetEarth(ConsoleApplication2.HipsterEngine engine, float x, float y, float radius) : base(engine)
        {
            X = x;
            Y = y;
            Radius = radius;
            
            RigidBody = (RigidBodyCircle) _engine.Physics.FactoryBody
                .CreateRigidCircle()
                .CreateCircleDef(0.3f, 0.8f, 0.0f, Radius)
                .CreateBodyDef(X, Y, 0, true, false)
                .Build(0);
            
            Paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                PathEffect = SKPathEffect.CreateDash(new[] { 10.0f, 10.0f }, 10),
                
                Color = new SKColor(210, 170, 0, 100)
             //   Shader = SKShader.CreateRadialGradient(new SKPoint(X, Y), Radius, new SKColor[]
             //   {
             //       new SKColor(50, 50, 50), 
             //       new SKColor(0, 140, 210, 100), 
             //   }, new float[] { 0.5f, Radius }, SKShaderTileMode.Repeat),
             //   Color = new SKColor(210, 170, 0, 100)
            };
            
            PaintEarth = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                StrokeWidth = 5,
                Color = new SKColor(100, 100, 100, 100)
            };
            
            PaintPlanet = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                StrokeWidth = 3,
                Color = new SKColor(80, 40, 0, 150)
            };
            
            PaintCore = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                StrokeWidth = 3,
                Color = new SKColor(50, 50, 50)
            };
            
            PaintCoreBorder = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                StrokeWidth = 3,
                Color = new SKColor(50, 100, 170, 100)
            };
        }

        public override void Draw()
        {
            TimeAtmosfereRadius += 0.01f;
            var scale = (float) ( Math.Cos(Math.Sin(TimeAtmosfereRadius*2) / 3));

            var x = RigidBody.GetX();
            var y = RigidBody.GetY();
            
            _canvas.Save();
            _canvas.Translate(x, y);
            _canvas.RotateRadians(TimeAtmosfereRadius / 10.0f);
            _canvas.Scale(scale, scale);
            _canvas.DrawCircle(0, 0, Radius*2 - Radius / 2, Paint);
            _canvas.Restore();
            _canvas.DrawCircle(x, y, Radius, PaintPlanet);
            _canvas.DrawCircle(x, y, Radius, PaintEarth);
            _canvas.DrawCircle(x, y, Radius - Radius/3, PaintCoreBorder);
            _canvas.DrawCircle(x, y, Radius - Radius/3-1, PaintCore);
        }
    }
}