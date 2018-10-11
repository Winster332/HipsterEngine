using HipsterEngine.Core.UI.Components.Screens;

namespace HipsterEngine.Core.Configurations
{
    public class Builder
    {
        private double _updatePerSecond;
        private double _framesPerSecond;
        private IStartup _startup;
        
        private Builder(IStartup startup)
        {
            _startup = startup;
        }

        public static Builder Create(IStartup startup, int width, int height)
        {
            var builder = new Builder(startup);
            builder._startup.Engine = builder._startup.CreateEngine(width, height);

            return builder;
        }

        public Builder SetTargetFPS(double updatePerSecond, double framesPerSecond)
        {
            _updatePerSecond = updatePerSecond;
            _framesPerSecond = framesPerSecond;

            return this;
        }

        public void Run(Screen screen)
        {
            _startup.Engine.SetStartScreen(screen);
            _startup.Run(_updatePerSecond, _framesPerSecond);
        }
    }
}