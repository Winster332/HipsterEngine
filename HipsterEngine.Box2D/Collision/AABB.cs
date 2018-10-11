using Box2DX.Common;

namespace Box2DX.Collision
{
    public struct AABB
    {
        public Vec2 LowerBound;
        public Vec2 UpperBound;

        public bool IsValid
        {
            get
            {
                Vec2 vec2 = this.UpperBound - this.LowerBound;
                return (double) vec2.X >= 0.0 && (double) vec2.Y >= 0.0 && this.LowerBound.IsValid && this.UpperBound.IsValid;
            }
        }
    }
}