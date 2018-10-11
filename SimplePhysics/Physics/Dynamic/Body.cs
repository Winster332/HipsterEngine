using SimplePhysics.Physics.Collision;

namespace SimplePhysics.Physics.Dynamic
{
    public class Body
    {
        public Shape Shape { get; set; }

        public void Step()
        {
            Shape.Position += Shape.Velacity;
        }
    }
}