using FlatPlant.Screens;
using HipsterEngine.Core.Configurations;
using HipsterEngine.Core.Desktop;

namespace FlatPlanet.Desktop
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Builder.Create(new HipsterStartup(), 1024, 720)
                .SetTargetFPS(60, 60)
                .Run(new GameScreen());
        }
    }
}