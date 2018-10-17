using System.Collections.Generic;
using Box2DX.Common;
using FlatPlant.Models.Planets;
using FlatPlant.Models.Robots.Arms.Bullets;
using FlatPlant.Models.Robots.Bodies;
using HipsterEngine.Core.Particles;
using SkiaSharp;
using Math = System.Math;

namespace FlatPlant.Models.Robots.Arms
{
    public class Gun1 : IArms
    {
        public float Angle { get; set; }
        public SKPaint Paint { get; set; }
        private HipsterEngine.Core.HipsterEngine _engine;
        private IBody _body;
        public float Width { get; set; }
        public float Height { get; set; }
        public List<GunBullet> Bullets { get; set; }
        private IPlanet _planet;
        
        public void Build(HipsterEngine.Core.HipsterEngine engine, IPlanet planet, IBody body)
        {
            _planet = planet;
            _engine = engine;
            _body = body;
            Width = 10;
            Height = 40;
            Bullets = new List<GunBullet>();
            
            Paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = new SKColor(30, 30, 30)
            };
            SetAngleRad(180);
        }

        public void Attack()
        {
            var a = _body.RigidBody.GetBody().GetAngle() + Angle; // + (-45.0f / (180.0f * Math.PI));
            var x = (float) Math.Cos(a + (float) Math.PI / 2);
            var y = (float) Math.Sin(a + (float) Math.PI / 2);

            var dx = _body.RigidBody.GetX();
            var dy = _body.RigidBody.GetY();


            var b = new GunBullet(_engine, dx + x, dy + y, 5);
            b.RigidBody.GetBody().ApplyImpulse(new Vec2(x * 10, y * 10), b.RigidBody.GetBody().GetPosition());
            Bullets.Add(b);
        }

        public void SetAngleRad(float value)
        {
            Angle = value / 180.0f * (float) Math.PI;
        }

        public void Update()
        {
        }

        public void Draw()
        {
         //   Angle++;
            _engine.Surface.Canvas.Save();
            _engine.Surface.Canvas.Translate(_body.RigidBody.GetX(), _body.RigidBody.GetY());
            _engine.Surface.Canvas.DrawCircle(0, 0, 10, Paint);
            _engine.Surface.Canvas.RotateRadians(_body.RigidBody.GetBody().GetAngle() + Angle);
            _engine.Surface.Canvas.DrawRect(-Width / 2, 0, Width, Height, Paint);
            _engine.Surface.Canvas.Restore();

            Bullets.ForEach(b =>
            {
                var forceVector = _planet.RigidBody.GetBody().GetPosition() - b.RigidBody.GetBody().GetPosition();
                b.RigidBody.GetBody().ApplyForce(forceVector, b.RigidBody.GetBody().GetPosition());

                if (b.RigidBody.GetBody().GetLinearVelocity().X > 0.3f || b.RigidBody.GetBody().GetLinearVelocity().Y > 0.3f)
                {
                    var ps = (ParticlesControllerFire) _engine.Particles.GetSystem(typeof(ParticlesControllerFire));
                    ps.AddBlood(b.RigidBody.GetX() - _engine.Surface.Canvas.Camera.X,
                        b.RigidBody.GetY() + _engine.Surface.Canvas.Camera.Y, new Vec2(), 5);
                }

                b.Draw(_engine.Surface.Canvas, new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = new SKColor(0, 0, 0)
                });
            });
        }
    }
}