using System;

namespace HipsterEngine.Core.UI.Animations
{
    public interface IAnimation<T> : IDisposable
    {
        TimeWatch Timer { get; set; }
        T From { get; set; }
        T Target { get; set; }
        T CurrentValue { get; set; }
        T Step { get; set; }
        void Start(T from, T target, T step);
        void Stop();
        void Update();
    }
}