using SimplePhysics.Physics.Common;

namespace SimplePhysics.Physics.Collision
{
    public abstract class Shape
    {
        public Vec2 Position { get; set; }
        public Vec2 Velacity { get; set; }

        public float X
        {
            get => Position.X;
            set => Position.X = value;
        }
        
        public float Y
        {
            get => Position.Y;
            set => Position.Y = value;
        }

        public Shape()
        {
            Velacity = new Vec2(0, 0);
            Position = new Vec2(0, 0);
        }

        public abstract bool Intersect(Shape shape);
    }
}