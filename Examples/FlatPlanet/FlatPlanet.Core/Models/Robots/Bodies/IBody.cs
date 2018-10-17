using Box2DX.Common;
using HipsterEngine.Core.Physics.Bodies;

namespace FlatPlant.Models.Robots.Bodies
{
    public interface IBody
    {
        RigidBody RigidBody { get; set; }
        IBody Build(HipsterEngine.Core.HipsterEngine engine, float x, float y, float angle, float size);
        Vec2[] PointsJoints { get; set; }
        void Update();
        void PlantCorn();
        void Draw();
    }
}