using System;

namespace SimplePhysics.Physics.Common
{
    public class Vec2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vec2()
        {
            X = 0;
            Y = 0;
        }

        public Vec2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vec2 operator +(Vec2 v1, Vec2 v2) => new Vec2(v1.X + v2.X, v1.Y + v2.Y);
        public static Vec2 operator -(Vec2 v1, Vec2 v2) => new Vec2(v1.X - v2.X, v1.Y - v2.Y);
        public static Vec2 operator /(Vec2 v1, Vec2 v2) => new Vec2(v1.X / v2.X, v1.Y / v2.Y);
        public static Vec2 operator *(Vec2 v1, Vec2 v2) => new Vec2(v1.X * v2.X, v1.Y * v2.Y);
        public static Vec2 operator *(Vec2 v1, float s) => new Vec2(v1.X * s, v1.Y * s);

        public float Length()
        {
            return (float) System.Math.Sqrt(this.X * this.X + this.Y * this.Y);
        }

        public static Vec2 Normalize(Vec2 v)
        {
            var vec = new Vec2();
            var length = v.Length();

            if (length > 0)
            {
                vec.X = v.X / length;
                vec.Y = v.Y / length;
            }

            return vec;
        }
        
        public static float Distance(Vec2 a, Vec2 b)
        {
            return (float) Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
    }
}