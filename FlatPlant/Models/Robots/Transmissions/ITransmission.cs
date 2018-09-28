using FlatPlant.Models.Planets;
using FlatPlant.Models.Robots.Bodies;

namespace FlatPlant.Models.Robots.Transmissions
{
    public interface ITransmission
    {
        ITransmission Build(ConsoleApplication2.HipsterEngine engine, IPlanet planet, float x, float y, float angle, float size);
        ITransmission FixedBody(IBody body, float x, float y);
        void Move(float value);
        void Update();
        void Draw();
    }
}