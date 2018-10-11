namespace Box2DX.Collision
{
    public class ShapeDef
    {
        public ShapeType Type;
        public object UserData;
        public float Friction;
        public float Restitution;
        public float Density;
        public bool IsSensor;
        public FilterData Filter;

        public ShapeDef()
        {
            this.Type = ShapeType.UnknownShape;
            this.UserData = (object) null;
            this.Friction = 0.2f;
            this.Restitution = 0.0f;
            this.Density = 0.0f;
            this.Filter.CategoryBits = (ushort) 1;
            this.Filter.MaskBits = ushort.MaxValue;
            this.Filter.GroupIndex = (short) 0;
            this.IsSensor = false;
        }
    }
}