using HipsterEngine.Core.Configurations;

namespace HipsterEngine.Core.Desktop
{
    public class HipsterStartup : IStartup
    {
        public GameWindowGPU Window { get; set; }
        public HipsterEngine Engine { get; set; }
        
        public HipsterEngine CreateEngine(int width, int height)
        {
            Window = new GameWindowGPU(width, height);

            return Window.Engine;
        }

        public void Run(double updatePerSecond, double framesPerSecond)
        {
            Window.Run(updatePerSecond, framesPerSecond);
        }

        public void Dispose()
        {
            Window.Dispose();
        }
    }
}