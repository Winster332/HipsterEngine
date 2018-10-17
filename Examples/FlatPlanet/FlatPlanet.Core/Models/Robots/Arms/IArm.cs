using FlatPlant.Models.Planets;
using FlatPlant.Models.Robots.Bodies;

namespace FlatPlant.Models.Robots.Arms
{
    public interface IArms
    {
        void Build(HipsterEngine.Core.HipsterEngine engine, IPlanet planet, IBody body);
        void Attack();
        void Update();
        void Draw();
    }
}