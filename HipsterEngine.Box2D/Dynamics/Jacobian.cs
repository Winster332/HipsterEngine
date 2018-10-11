using Box2DX.Common;

namespace Box2DX.Dynamics
{
    public struct Jacobian
    {
        public Vec2 Linear1;
        public float Angular1;
        public Vec2 Linear2;
        public float Angular2;

        public void SetZero()
        {
            this.Linear1.SetZero();
            this.Angular1 = 0.0f;
            this.Linear2.SetZero();
            this.Angular2 = 0.0f;
        }

        public void Set(Vec2 x1, float a1, Vec2 x2, float a2)
        {
            this.Linear1 = x1;
            this.Angular1 = a1;
            this.Linear2 = x2;
            this.Angular2 = a2;
        }

        public float Compute(Vec2 x1, float a1, Vec2 x2, float a2)
        {
            return (float) ((double) Vec2.Dot(this.Linear1, x1) + (double) this.Angular1 * (double) a1 + (double) Vec2.Dot(this.Linear2, x2) + (double) this.Angular2 * (double) a2);
        }
    }
}