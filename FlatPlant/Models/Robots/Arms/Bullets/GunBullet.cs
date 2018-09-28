using ConsoleApplication2;
using ConsoleApplication2.Graphics;
using ConsoleApplication2.Physics.Bodies;
using SkiaSharp;

namespace FlatPlant.Models.Robots.Arms.Bullets
{
    public class GunBullet
    {
        public RigidBodyCircle RigidBody { get; set; }
        public float Radius { get; set; }
        
        public GunBullet(ConsoleApplication2.HipsterEngine engine, float x, float y, float radius)
        {
            Radius = radius;

            RigidBody = (RigidBodyCircle) engine.Physics.FactoryBody
                .CreateRigidCircle()
                .CreateCircleDef(0.2f, 0.2f, 0.2f, Radius)
                .CreateBodyDef(x, y, 0, true, true)
                .Build(1);
        }

        public void Draw(Canvas canvas, SKPaint paint)
        {
            canvas.DrawCircle(RigidBody.GetX(), RigidBody.GetY(), Radius, paint);
        }
    }
}