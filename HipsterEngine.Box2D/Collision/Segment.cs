using Box2DX.Common;

namespace Box2DX.Collision
{
    public struct Segment
    {
        public Vec2 P1;
        public Vec2 P2;

        public bool TestSegment(out float lambda, out Vec2 normal, Segment segment, float maxLambda)
        {
            lambda = 0.0f;
            normal = new Vec2();
            Vec2 p1 = segment.P1;
            Vec2 a1 = segment.P2 - p1;
            Vec2 b = Vec2.Cross(this.P2 - this.P1, 1f);
            float num1 = 100f * Settings.FLT_EPSILON;
            float num2 = -Vec2.Dot(a1, b);
            if ((double) num2 > (double) num1)
            {
                Vec2 a2 = p1 - this.P1;
                float num3 = Vec2.Dot(a2, b);
                if (0.0 <= (double) num3 && (double) num3 <= (double) maxLambda * (double) num2)
                {
                    float num4 = (float) (-(double) a1.X * (double) a2.Y + (double) a1.Y * (double) a2.X);
                    if (-(double) num1 * (double) num2 <= (double) num4 && (double) num4 <= (double) num2 * (1.0 + (double) num1))
                    {
                        float num5 = num3 / num2;
                        double num6 = (double) b.Normalize();
                        lambda = num5;
                        normal = b;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}