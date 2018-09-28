using Box2DX.Common;
using Math = System.Math;

namespace FlatPlant.Extensions
{
    public static class ExtensionsVec2
    {
        public static Vec2 GetNormilizeVector(this Vec2 v)
        {
            return new Vec2(v.X / v.Length(), v.Y / v.Length());
        }
        
        public static Vec2 Rotate(this Vec2 v, float angle)
        {
            var x = v.X * (float) Math.Cos(angle) - v.Y * (float) Math.Sin(angle);
            var y = v.X * (float) Math.Sin(angle) + v.Y * (float) Math.Cos(angle);
            
            return new Vec2(x, y);
        }
        
        public static Vec2 RotateGrad(this Vec2 v, float angle, float radius)
        {
            angle /= 180 * (float)Math.PI;

            var x = v.X * (float) Math.Cos(angle) * radius - v.Y * (float) Math.Sin(angle) * radius;
            var y = v.X * (float) Math.Sin(angle) * radius + v.Y * (float) Math.Cos(angle) * radius;
            
            return new Vec2(x, y);
        }

        public static float GetAngle(this Vec2 v)
        {
            return (float) Math.Atan2(v.Y, v.X);
        }

        public static Vec2 Mult(this Vec2 v, float value)
        {
            return new Vec2(v.X * value, v.Y * value);
        }
    }
}