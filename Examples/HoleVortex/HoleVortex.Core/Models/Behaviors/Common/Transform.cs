using Box2DX.Common;
using HipsterEngine.Core.Graphics;

namespace HoleVortex.Core.Models.Behaviors.Common
{
    public class Transform
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Angle { get; set; }
        public Vec2 Scale { get; set; }

        public Transform()
        {
            Angle = 0;
            Scale = new Vec2(1, 1);
        }

        public void Bind(Canvas canvas)
        {
            canvas.Save();
            canvas.Translate(X, Y);
            canvas.Scale(Scale.X, Scale.Y);
            canvas.RotateRadians(Angle);
        }

        public void Unbind(Canvas canvas)
        {
            canvas.Restore();
        }
    }
}