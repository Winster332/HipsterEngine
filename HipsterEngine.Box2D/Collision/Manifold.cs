using Box2DX.Common;

namespace Box2DX.Collision
{
    public class Manifold
    {
        public ManifoldPoint[] Points = new ManifoldPoint[Settings.MaxManifoldPoints];
        public Vec2 Normal;
        public int PointCount;

        public Manifold()
        {
            for (int index = 0; index < Settings.MaxManifoldPoints; ++index)
                this.Points[index] = new ManifoldPoint();
        }

        public Manifold Clone()
        {
            Manifold manifold = new Manifold();
            manifold.Normal = this.Normal;
            manifold.PointCount = this.PointCount;
            int length = this.Points.Length;
            ManifoldPoint[] manifoldPointArray = new ManifoldPoint[length];
            for (int index = 0; index < length; ++index)
                manifoldPointArray[index] = this.Points[index].Clone();
            manifold.Points = manifoldPointArray;
            return manifold;
        }
    }
}