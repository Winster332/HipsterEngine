using Box2DX.Common;
using ConsoleApplication2.Physics.Bodies;
using FlatPlant.Models.Planets;
using FlatPlant.Models.Robots.Bodies;

namespace FlatPlant.Models.Robots.Transmissions
{
    public class TwoWheels : ITransmission
    {
        public Wheel Wheel1 { get; set; }
        public Wheel Wheel2 { get; set; }
        private ConsoleApplication2.HipsterEngine _engine;

        public TwoWheels()
        {
        }

        public ITransmission Build(ConsoleApplication2.HipsterEngine engine, IPlanet planet, float x, float y, float angle, float size)
        {
            _engine = engine;
            Wheel1 = (Wheel) new Wheel().Build(_engine, planet, x - size - 10, y, angle, size);
            Wheel2 = (Wheel) new Wheel().Build(_engine, planet, x + size + 10, y, angle, size);

            var distance = Vec2.Distance(Wheel1.RigidBody.GetBody().GetPosition(),
                Wheel2.RigidBody.GetBody().GetPosition());
            Wheel1.RigidBody.JointDistance(Wheel2.RigidBody, 0, 0, 0, 0, false, 1, distance);

            return this;
        }

        public ITransmission FixedBody(IBody body, float x, float y)
        {
            Wheel1.FixedBody(body, -x, y);
            Wheel2.FixedBody(body, x, y);
            
            return this;
        }

        public void Move(float value)
        {
            Wheel1.Move(value);
            Wheel2.Move(value);
        }

        public void Update()
        {
            Wheel1.Update();
            Wheel2.Update();
        }

        public void Draw()
        {
            Wheel1.Draw();
            Wheel2.Draw();
        }
    }
}