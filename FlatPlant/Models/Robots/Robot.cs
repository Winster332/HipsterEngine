using System;
using ConsoleApplication2.Graphics;
using FlatPlant.Extensions;
using FlatPlant.Models.Planets;
using FlatPlant.Models.Robots.Arms;
using FlatPlant.Models.Robots.Bodies;
using FlatPlant.Models.Robots.Transmissions;

namespace FlatPlant.Models.Robots
{
    public class Robot : IRobot
    {
        public ITransmission Transmission { get; set; }
        public IBody Body { get; set; }
        public IArms Arms { get; set; }
        private ConsoleApplication2.HipsterEngine _engine;
        private IPlanet _planet;

        public Robot(ConsoleApplication2.HipsterEngine engine, IPlanet planet)
        {
            _engine = engine;
            _planet = planet;
        }

        public IRobot Build(float x, float y, float angle, float size)
        {
            
            Body = Body.Build(_engine, x, y, 0, size);
            Transmission = Transmission.Build(_engine, _planet, x, y, 0, size);

            for (var i = 0; i < Body.PointsJoints.Length; i++)
            {
                var point = Body.PointsJoints[i];
                
                Transmission.FixedBody(Body, point.X, point.Y);
            }
            
            Arms.Build(_engine, _planet, Body);
            
            return this;
        }

        public IRobot Initialize(ITransmission transmission, IBody body, IArms arms)
        {
            Transmission = transmission;
            Body = body;
            Arms = arms;

            return this;
        }

        public void SendCommands(ICommand command)
        {
        }

        public void Update(double time, float dt)
        {
            Transmission.Update();
            Body.Update();
            Arms.Update();
        }

        public void Draw(Canvas canvas)
        {
            Transmission.Draw();
            Body.Draw();
            Arms.Draw();
        }
    }
}