using HipsterEngine.Core.Physics.Bodies;

namespace FlatPlant.Models.Planets
{
    public interface IPlanet
    {
        float X { get; set; }
        float Y { get; set; }
        RigidBody RigidBody { get; set; }
    }
}