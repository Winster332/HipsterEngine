namespace Box2DX.Common
{
    public struct Vec3
    {
        public float X;
        public float Y;
        public float Z;

        public Vec3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public void SetZero()
        {
            this.X = 0.0f;
            this.Y = 0.0f;
            this.Z = 0.0f;
        }

        public void Set(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static float Dot(Vec3 a, Vec3 b)
        {
            return (float) ((double) a.X * (double) b.X + (double) a.Y * (double) b.Y + (double) a.Z * (double) b.Z);
        }

        public static Vec3 Cross(Vec3 a, Vec3 b)
        {
            return new Vec3((float) ((double) a.Y * (double) b.Z - (double) a.Z * (double) b.Y), (float) ((double) a.Z * (double) b.X - (double) a.X * (double) b.Z), (float) ((double) a.X * (double) b.Y - (double) a.Y * (double) b.X));
        }

        public static Vec3 operator -(Vec3 v)
        {
            return new Vec3(-v.X, -v.Y, -v.Z);
        }

        public static Vec3 operator +(Vec3 v1, Vec3 v2)
        {
            return new Vec3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vec3 operator -(Vec3 v1, Vec3 v2)
        {
            return new Vec3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vec3 operator *(Vec3 v, float s)
        {
            return new Vec3(v.X * s, v.Y * s, v.Z * s);
        }

        public static Vec3 operator *(float s, Vec3 v)
        {
            return new Vec3(v.X * s, v.Y * s, v.Z * s);
        }
    }
}
