using System;
using Box2DX.Common;
using HipsterEngine.Core.Graphics;
using HipsterEngine.Core.Particles;
using HoleVortex.Core.Models.Behaviors;
using HoleVortex.Core.Models.Behaviors.Common;
using HoleVortex.Core.Models.Planets;
using HoleVortex.Core.Particles;
using HoleVortex.Core.Screens;
using SkiaSharp;
using Math = System.Math;

namespace HoleVortex.Core.Models.Meshes
{
    public delegate void EndGameEventHandler(GameResult result);
    
    public class Triangle : Behavior, IDisposable
    {
        public float Radius { get; set; }
        public Vec2 A { get; set; }
        public Vec2 B { get; set; }
        public Vec2 C { get; set; }
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
            Transform.X = x;
            Transform.Y = y;
            base.Paint += OnPaint;
            base.Update += OnUpdate;
            MaxMinHeight = Transform.Y - _engine.Surface.Height / 4;
            Radius = radius;
            A = new Vec2(-Radius, Radius);
            B = new Vec2(Radius, Radius);
            C = new Vec2(0, -Radius);
            VelacityX = 0;
            VelacityY = 0;
            
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
        
        private void OnUpdate()
        {
            Transform.X += VelacityX;
            Transform.Y += VelacityY;

            if (CurrentPlanet != null)
            {
                Transform.Angle = CurrentPlanet.Transform.Angle - CurrentPlanet.CorrectAngle;

                var x = (float) Math.Cos(Transform.Angle - Math.PI / 2);
                var y = (float) Math.Sin(Transform.Angle - Math.PI / 2);

                x *= (CurrentPlanet.Radius + Radius - 2);
                y *= (CurrentPlanet.Radius + Radius - 2);
                
                Transform.X = CurrentPlanet.Transform.X + x;
                Transform.Y = CurrentPlanet.Transform.Y + y;

          //      _engine.Surface.Canvas.Camera.SetAngleTarget(-Angle, CurrentPlanet.X, CurrentPlanet.Y);
        //        _engine.Surface.Canvas.Camera.Angle = -CurrentPlanet.Angle;
        //        _engine.Surface.Canvas.Camera.CenterRotation = new Vec2(CurrentPlanet.X, CurrentPlanet.Y);
            }
            else
            {
                if (VelacityY <= 0)
                    _engine.Surface.Canvas.Camera.SetTarget(0,
                        -Transform.Y + (_engine.Surface.Height) - _engine.Surface.Height / 4);
                //     _engine.Surface.Canvas.Camera.SetAngleTarget(-Angle, X, Y);

            //    var ps = (ParticlesControllerFire) _engine.Particles.GetSystem(typeof(TriangleParticlesController));
            //    ps.AddBlood(Transform.X, Transform.Y, new Vec2(), 1);
            }
        }

        private void OnPaint()
        {
            _engine.Surface.Canvas.DrawPath(Path, Paint);
        }

        public void SetPlanet(Planet planet)
        {
            CurrentPlanet = planet;
            MaxMinHeight = planet.Transform.Y;
            
            var ps = (ParticlesControllerFire) _engine.Particles.GetSystem(typeof(ParticlesControllerFire));
            ps.AddBlood(CurrentPlanet.Transform.X, CurrentPlanet.Transform.Y, new Vec2(), 20);
            
            _engine.Surface.Canvas.Camera.SetTarget(0, -CurrentPlanet.Transform.Y + (_engine.Surface.Height) - _engine.Surface.Height / 4);
        }

        public void Jump()
        {
            CurrentPlanet = null;

            var x = (float) Math.Cos(Transform.Angle - Math.PI / 2);
            var y = (float) Math.Sin(Transform.Angle - Math.PI / 2);

            x *= 5;
            y *= 5;

            VelacityX = x;
            VelacityY = y;
        }

        public bool Intersect(Planet planet)
        {
            var distance = (float) Math.Sqrt(Math.Pow(planet.Transform.X - Transform.X, 2) + Math.Pow(planet.Transform.Y - Transform.Y, 2));
            var totalRadius = Radius + planet.Radius;

            if (distance <= totalRadius && planet.IsCheck == false)
            {
                var angle = (float) Math.Atan2(Transform.Y - planet.Transform.Y, Transform.X - planet.Transform.X);
                angle += (float)Math.PI / 2 - planet.Transform.Angle;

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