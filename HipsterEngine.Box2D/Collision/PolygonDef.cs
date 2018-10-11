using Box2DX.Common;

namespace Box2DX.Collision
{
    public class PolygonDef : ShapeDef
    {
        public Vec2[] Vertices = new Vec2[Settings.MaxPolygonVertices];
        public int VertexCount;

        public PolygonDef()
        {
            this.Type = ShapeType.PolygonShape;
            this.VertexCount = 0;
        }

        public void SetAsBox(float hx, float hy)
        {
            this.VertexCount = 4;
            this.Vertices[0].Set(-hx, -hy);
            this.Vertices[1].Set(hx, -hy);
            this.Vertices[2].Set(hx, hy);
            this.Vertices[3].Set(-hx, hy);
        }

        public void SetAsBox(float hx, float hy, Vec2 center, float angle)
        {
            this.SetAsBox(hx, hy);
            XForm T = new XForm();
            T.Position = center;
            T.R.Set(angle);
            for (int index = 0; index < this.VertexCount; ++index)
                this.Vertices[index] = Math.Mul(T, this.Vertices[index]);
        }
    }
}