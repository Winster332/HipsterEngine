using System;

namespace HipsterEngine.Core.Configurations
{
    public interface IStartup : IDisposable
    {
        HipsterEngine Engine { get; set; }
        HipsterEngine CreateEngine(int width, int height);
        void Run(double updatePerSecond, double framesPerSecond);
    }
}