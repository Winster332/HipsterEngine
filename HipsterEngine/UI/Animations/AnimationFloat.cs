namespace ConsoleApplication2.UI.Animations
{
    public class AnimationFloat : IAnimation<float>
    {
        public TimeWatch Timer { get; set; }
        public float From { get; set; }
        public float Target { get; set; }
        public float CurrentValue { get; set; }
        public float Step { get; set; }

        public AnimationFloat()
        {
            Timer = new TimeWatch();
            Timer.Tick += TimerOnTick;
        }
        
        public void Start(float from, float target, float step)
        {
            From = from;
            Target = target;
            Step = step;
            CurrentValue = From;

            Timer.Start(1);
        }

        private void TimerOnTick(int counttick)
        {
            if (From < Target)
            {
                if (CurrentValue < Target)
                {
                    CurrentValue += Step;
                }
                else
                {
                    Stop();
                }
            }
            else
            {
                if (CurrentValue > Target)
                {
                    CurrentValue -= Step;
                }
                else
                {
                    Stop();
                }
            }
        }


        public void Stop()
        {
            Timer.Stop();
        }

        public void Update() => Timer.Update();

        public void Dispose()
        {
        }
    }
}