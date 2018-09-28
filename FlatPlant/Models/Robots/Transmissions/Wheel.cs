using Box2DX.Common;
using ConsoleApplication2;
using ConsoleApplication2.Physics.Bodies;
using FlatPlant.Models.Planets;
using FlatPlant.Models.Robots.Bodies;
using SkiaSharp;

namespace FlatPlant.Models.Robots.Transmissions
{
    public class Wheel : ITransmission
    {
        public float X => RigidBody.GetX();
        public float Y => RigidBody.GetY();
        public RigidBodyCircle RigidBody { get; set; }
        public SKPaint Paint { get; set; }
        public SKPaint Paint1 { get; set; }
        public float Radius { get; set; }
        
        protected ConsoleApplication2.HipsterEngine _engine;
        protected IPlanet Planet { get; set; }

        public Wheel()
        {
        }

        public ITransmission Build(ConsoleApplication2.HipsterEngine engine, IPlanet planet, float x, float y, float angle, float size)
        {
            _engine = engine;
            Planet = planet;
            Radius = size;

            RigidBody = (RigidBodyCircle) _engine.Physics.FactoryBody
                .CreateRigidCircle()
                .CreateCircleDef(0.2f, 0.8f, 50, Radius)
                .CreateBodyDef(x, y, angle, true, false)
                .Build(1f);
            
            Paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                PathEffect = SKPathEffect.CreateDash(new[] { 10.0f, 10.0f }, 10),
                StrokeWidth = 5,
                Color = new SKColor(100, 70, 30)
            };
            
            Paint1 = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = new SKColor(150, 100, 50)
            };

            return this;
        }

        public ITransmission FixedBody(IBody body, float x, float y)
        {
            var distance = Vec2.Distance(RigidBody.GetBody().GetPosition(), body.RigidBody.GetBody().GetPosition());
            //body.RigidBody.JointDistance(RigidBody, x, y, 0, 0, false, 5, distance);
            RigidBody.JointRevolute(body.RigidBody, 0, 0, false, true, false);
            
            return this;
        }

        public void Move(float value)
        {
            RigidBody.GetBody().SetAngularVelocity(value);
        }

        public void Update()
        {
            var forceVector = Planet.RigidBody.GetBody().GetPosition() - RigidBody.GetBody().GetPosition();
            //forceVector.X /= 10;
            //forceVector.Y /= 10;
            RigidBody.GetBody().ApplyForce(forceVector, RigidBody.GetBody().GetPosition());
        }

        public void Draw()
        {
            _engine.Surface.Canvas.Save();
            _engine.Surface.Canvas.Translate(X, Y);
            _engine.Surface.Canvas.RotateRadians(RigidBody.GetBody().GetAngle());
            _engine.Surface.Canvas.DrawCircle(0, 0, Radius, Paint);
            _engine.Surface.Canvas.DrawCircle(0, 0, Radius, Paint1);
            _engine.Surface.Canvas.Restore();
        }
    }
}