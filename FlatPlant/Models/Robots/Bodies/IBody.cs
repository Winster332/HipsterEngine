using Box2DX.Common;
using ConsoleApplication2.Physics.Bodies;

namespace FlatPlant.Models.Robots.Bodies
{
    public interface IBody
    {
        RigidBody RigidBody { get; set; }
        IBody Build(ConsoleApplication2.HipsterEngine engine, float x, float y, float angle, float size);
        Vec2[] PointsJoints { get; set; }
        void Update();
        void PlantCorn();
        void Draw();
    }
}