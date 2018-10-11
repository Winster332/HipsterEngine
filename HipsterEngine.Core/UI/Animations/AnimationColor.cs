using SkiaSharp;

namespace HipsterEngine.Core.UI.Animations
{
    public class AnimationColor : IAnimation<SKColor>
    {
        public TimeWatch Timer { get; set; }
        public SKColor From { get; set; }
        public SKColor Target { get; set; }
        public SKColor CurrentValue { get; set; }
        public SKColor Step { get; set; }

        public AnimationColor()
        {
            Timer = new TimeWatch();
            Timer.Tick += TimerOnTick;
        }
        
        private void TimerOnTick(int counttick)
        {
            if (CurrentValue.Alpha < Target.Alpha)
            {
                CurrentValue.WithAlpha((byte)(CurrentValue.Alpha + Step.Alpha));
                CurrentValue.WithRed((byte)(CurrentValue.Red + Step.Red));
                CurrentValue.WithGreen((byte)(CurrentValue.Green + Step.Green));
                CurrentValue.WithBlue((byte)(CurrentValue.Blue + Step.Blue));
            }
            else
            {
                Stop();
            }
        }
        
        public void Update() => Timer.Update();
        
        public void Start(SKColor from, SKColor target, SKColor step)
        {
            From = from;
            Target = target;
            Step = step;
            CurrentValue = From;

            Timer.Start(1);
        }

        public void Stop()
        {
            Timer.Stop();
        }

        public void Dispose()
        {
        }
    }
}