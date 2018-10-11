namespace SimplePhysics.Physics.Common
{
    public class AABB
    {
        public Vec2 Max { get; set; }
        public Vec2 Min { get; set; }

        public AABB()
        {
            Max = new Vec2(0, 0);
            Min = new Vec2(0, 0);
        }

        public AABB(Vec2 max, Vec2 min)
        {
            Max = max;
            Min = min;
        }

        public AABB(float maxX, float maxY, float minX, float minY)
        {
            Max = new Vec2(maxX, maxY);
            Min = new Vec2(minX, minY);
        }

        public static bool Instersect(AABB a, AABB b)
        {
            if (a.Max.X < b.Min.X || a.Min.X > b.Max.X) return false;
            if (a.Max.Y < b.Min.Y || a.Min.Y > b.Max.Y) return false;

            return true;
        }
    }
}