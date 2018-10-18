using System;
using Box2DX.Common;
using HipsterEngine.Core.Particles;
using HoleVortex.Core.Models.Planets;
using HoleVortex.Core.Screens;
using SkiaSharp;
using Math = System.Math;

namespace HoleVortex.Core.Models
{
    public delegate void EndGameEventHandler(GameResult result);
    
    public class Triangle : IDisposable
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Radius { get; set; }
        public Vec2 A { get; set; }
        public Vec2 B { get; set; }
        public Vec2 C { get; set; }
        public float Angle { get; set; }
        public float VelacityX { get; set; }
        public float VelacityY { get; set; }
        public Planet CurrentPlanet { get; set; }
        public event EventHandler IncrementBalls;
        private HipsterEngine.Core.HipsterEngine _engine;
        private float MaxMinHeight { get; set; } = 0;
        public SKPaint Paint { get; set; }
        public SKPath Path { get; set; }
        
        public Triangle(HipsterEngine.Core.HipsterEngine engine, float x, float y, float radius)
        {
            _engine = engine;
            X = x;
            Y = y;
            MaxMinHeight = Y - _engine.Surface.Height / 4;
            Radius = radius;
            A = new Vec2(-Radius, Radius);
            B = new Vec2(Radius, Radius);
            C = new Vec2(0, -Radius);
            VelacityX = 0;
            VelacityY = 0;
            Angle = 0;
            
            Paint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = new SKColor(150, 150, 150)
            };
            
            Path = new SKPath();
            Path.MoveTo(A.X, A.Y);
            Path.LineTo(B.X, B.Y);
            Path.LineTo(C.X, C.Y);
            Path.Close();
        }

        public void SetPlanet(Planet planet)
        {
            CurrentPlanet = planet;
            MaxMinHeight = planet.Y;
            
            var ps = (ParticlesControllerFire) _engine.Particles.GetSystem(typeof(ParticlesControllerFire));
            ps.AddBlood(CurrentPlanet.X, CurrentPlanet.Y, new Vec2(), 20);
            
            _engine.Surface.Canvas.Camera.SetTarget(0, -CurrentPlanet.Y + (_engine.Surface.Height) - _engine.Surface.Height / 4);
        }

        public void Update()
        {
            X += VelacityX;
            Y += VelacityY;

            if (CurrentPlanet != null)
            {
                Angle = CurrentPlanet.Angle - CurrentPlanet.CorrectAngle;

                var x = (float) Math.Cos(Angle - Math.PI / 2);
                var y = (float) Math.Sin(Angle - Math.PI / 2);

                x *= (CurrentPlanet.Radius + Radius - 2);
                y *= (CurrentPlanet.Radius + Radius - 2);
                
                X = CurrentPlanet.X + x;
                Y = CurrentPlanet.Y + y;

          //      _engine.Surface.Canvas.Camera.SetAngleTarget(-Angle, CurrentPlanet.X, CurrentPlanet.Y);
        //        _engine.Surface.Canvas.Camera.Angle = -CurrentPlanet.Angle;
        //        _engine.Surface.Canvas.Camera.CenterRotation = new Vec2(CurrentPlanet.X, CurrentPlanet.Y);
            }
            else
            {
                if (VelacityY <= 0)
                    _engine.Surface.Canvas.Camera.SetTarget(0,
                        -Y + (_engine.Surface.Height) - _engine.Surface.Height / 4);
                //     _engine.Surface.Canvas.Camera.SetAngleTarget(-Angle, X, Y);

                var ps = (ParticlesControllerFire) _engine.Particles.GetSystem(typeof(ParticlesControllerFire));
                ps.AddBlood(X, Y, new Vec2(), 1);
            }
        }

        public void Draw()
        {
            
            _engine.Surface.Canvas.Save();
            _engine.Surface.Canvas.Translate(X, Y);
            _engine.Surface.Canvas.RotateRadians(Angle);
            _engine.Surface.Canvas.DrawPath(Path, Paint);
            _engine.Surface.Canvas.Restore();
        }

        public void Jump()
        {
            CurrentPlanet = null;

            var x = (float) Math.Cos(Angle - Math.PI / 2);
            var y = (float) Math.Sin(Angle - Math.PI / 2);

            x *= 5;
            y *= 5;

            VelacityX = x;
            VelacityY = y;
        }

        public bool Intersect(Planet planet)
        {
            var distance = (float) Math.Sqrt(Math.Pow(planet.X - X, 2) + Math.Pow(planet.Y - Y, 2));
            var totalRadius = Radius + planet.Radius;

            if (distance <= totalRadius && planet.IsCheck == false)
            {
                var angle = (float) Math.Atan2(Y - planet.Y, X - planet.X);
                angle += (float)Math.PI / 2 - planet.Angle;

            //    planet.AngularVelocity = 0.04f;
                planet.CorrectAngle = -angle;
            //    planet.Angle = angle;
                planet.IsCheck = true;
                SetPlanet(planet);

                ((GameScreen) _engine.Screens.CurrentScreen).LayoutTop.IncrementBalls();
                IncrementBalls?.Invoke(null, null);
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            Paint?.Dispose();
            Path?.Dispose();
        }
    }
}