using FlatPlant.Models.Robots.Arms;
using FlatPlant.Models.Robots.Bodies;
using FlatPlant.Models.Robots.Transmissions;
using HipsterEngine.Core.Graphics;

namespace FlatPlant.Models.Robots
{
    public interface IRobot
    {
        ITransmission Transmission { get; set; }
        IBody Body { get; set; }
        IArms Arms { get; set; }

        IRobot Initialize(ITransmission transmission, IBody body, IArms arms);
        IRobot Build(float x, float y, float angle, float size);
        void SendCommands(ICommand command);
        void Update(double time, float dt);
        void Draw(Canvas canvas);
    }
}