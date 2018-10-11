using Box2DX.Common;

namespace Box2DX.Dynamics
{
    public abstract class DebugDraw
    {
        protected DebugDraw.DrawFlags _drawFlags;

        public DebugDraw()
        {
            this._drawFlags = (DebugDraw.DrawFlags) 0;
        }

        public DebugDraw.DrawFlags Flags
        {
            get
            {
                return this._drawFlags;
            }
            set
            {
                this._drawFlags = value;
            }
        }

        public void AppendFlags(DebugDraw.DrawFlags flags)
        {
            this._drawFlags |= flags;
        }

        public void ClearFlags(DebugDraw.DrawFlags flags)
        {
            this._drawFlags &= ~flags;
        }

        public abstract void DrawPolygon(Vec2[] vertices, int vertexCount, Color color);

        public abstract void DrawSolidPolygon(Vec2[] vertices, int vertexCount, Color color);

        public abstract void DrawCircle(Vec2 center, float radius, Color color);

        public abstract void DrawSolidCircle(Vec2 center, float radius, Vec2 axis, Color color);

        public abstract void DrawSegment(Vec2 p1, Vec2 p2, Color color);

        public abstract void DrawXForm(XForm xf);

        [System.Flags]
        public enum DrawFlags
        {
            Shape = 1,
            Joint = 2,
            CoreShape = 4,
            Aabb = 8,
            Obb = 16, // 0x00000010
            Pair = 32, // 0x00000020
            CenterOfMass = 64, // 0x00000040
        }
    }
}

