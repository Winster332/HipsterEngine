using System.Diagnostics;
using Box2DX.Common;
using ConsoleApplication2.Physics.Bodies;
using OpenTK;
using Math = System.Math;

namespace ConsoleApplication2.Graphics
{
    public class Camera
    {
        public float X
        {
            get => _surface.Width / 2 + _x;
            set => _x = value;
        }

        public float Y
        {
            get => _surface.Height / 2 + _y;
            set => _y = -value;
        }
        
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        
        public float Angle { get; set; }
        public float TargetAngle { get; set; }
        public Vec2 CenterRotation { get; set; }
        public RigidBody TargetBody { get; set; }
        private float _x;
        private float _y;
        private Surface _surface;
        private Vec2 TargetPosition { get; set; }

        public Camera(Surface surface)
        {
            _surface = surface;
            X = 0;
            Y = 0;
            SetScale(0);
            Angle = 0;
            TargetPosition = new Vec2();
            CenterRotation = new Vec2();
            TargetAngle = 0;
        }

        public void SetScale(float scale)
        {
            ScaleX = scale;
            ScaleY = scale;
        }

        public void SetTargetBody(RigidBody body)
        {
            TargetBody = body;
        }

        public void Update()
        {
            if (TargetBody != null)
            {
                var x = -TargetBody.GetX();
                var y = TargetBody.GetY();

                X = x;
                Y = y;
            }
            
            var cameraPosition = new Vec2(X, Y);

            var lerp = Vector2.Lerp(new Vector2(cameraPosition.X - _surface.Width / 2, _surface.Height / 2 - cameraPosition.Y),
                new Vector2(TargetPosition.X - _surface.Width / 2, -TargetPosition.Y + _surface.Height / 2), 0.05f);
            X = lerp.X;
            Y = lerp.Y;
            Angle = Lerp(Angle, TargetAngle, 0.1f);
        }
        
        public float Lerp(float a, float b, float p)
        {
            return (b-a)*p + a;
           // return a + t * (b - a); 
        }

        public void SetTarget(float mX, float mY)
        {
            TargetPosition = new Vec2(mX, mY);
        }

        public void SetAngleTarget(float angle, float px, float py)
        {
            TargetAngle = angle;
            CenterRotation = new Vec2(px, py);
        }

        public void ApplyTargetX(float value)
        {
            TargetPosition = new Vec2(value + TargetPosition.X, TargetPosition.Y);
        }
    }
}