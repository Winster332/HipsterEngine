using Box2DX.Common;
using ConsoleApplication2.Physics.Bodies;
using FlatPlant.Models.Planets;
using SkiaSharp;

namespace FlatPlant.Models.Robots.Bodies
{
    public class Box2 : IBody
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public SKPaint Paint { get; set; }
        public SKPaint Paint1 { get; set; }
        public RigidBody RigidBody { get; set; }
        public Vec2[] PointsJoints { get; set; }
        
        protected ConsoleApplication2.HipsterEngine _engine;

        public Box2()
        {
        }

        public IBody Build(ConsoleApplication2.HipsterEngine engine, float x, float y, float angle, float size)
        {
            _engine = engine;
            Width = size * 2;
            Height = size * 2;
            PointsJoints = new Vec2[]
            {
                new Vec2(size, 0), 
            };

            RigidBody = (RigidBodyVertex) _engine.Physics.FactoryBody
                .CreateRigidVertex()
                .CreateBox(0.2f, 0.8f, Width, Height, 0.2f)
                .CreateBodyDef(x, y - size*2, angle, true, false)
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


        public void Update()
        {
        }

        public void PlantCorn()
        {
            _engine.Physics.FactoryBody
                .CreateRigidVertex()
                .CreateBox(0.2f, 0.8f, 10, 10, 0.2f)
                .CreateBodyDef(RigidBody.GetX(), RigidBody.GetY() - 20, 0, true, false)
                .Build(1f);
        }

        public void Draw()
        {
            _engine.Surface.Canvas.Save();
            _engine.Surface.Canvas.Translate(RigidBody.GetX(), RigidBody.GetY());
            _engine.Surface.Canvas.RotateRadians(RigidBody.GetBody().GetAngle());
            
            _engine.Surface.Canvas.DrawRect(-Width / 2, -Height / 2, Width, Height, Paint1);
            _engine.Surface.Canvas.DrawRect(-Width / 2, -Height / 2, Width, Height, Paint);
            
            _engine.Surface.Canvas.Restore();
        }
    }
}