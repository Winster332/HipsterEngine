using Box2DX.Collision;

namespace Box2DX.Dynamics
{
    public class NullContact : Contact
    {
        public override void Evaluate(ContactListener listener)
        {
        }

        public override Manifold[] GetManifolds()
        {
            return (Manifold[]) null;
        }
    }
}