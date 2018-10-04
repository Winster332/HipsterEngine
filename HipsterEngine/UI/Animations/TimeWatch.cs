namespace ConsoleApplication2.UI.Animations
{
    public delegate void TimerEventHandler(int countTick);
    
    public class TimeWatch
    {
        public event TimerEventHandler Tick;
        public event TimerEventHandler Complated;
        public int Timeout { get; set; }
        public int CountTicks { get; set; }
        public int MaxTicks { get; set; }
        
        private bool _enabled;
        private int _time { get; set; }
        private HipsterEngine _engine;

        public TimeWatch()
        {
            _engine = null;
            Timeout = 0;
            _enabled = false;
            _time = 0;
            CountTicks = 0;
            MaxTicks = -1;
        }
        
        public TimeWatch(HipsterEngine engine)
        {
            Timeout = 0;
            _enabled = false;
            _time = 0;
            CountTicks = 0;
            MaxTicks = -1;
            _engine = engine;

            _engine.Screens.CurrentScreen.Update += (time, dt) => Update();
        }

        public void Start(int timeout, int maxTicks = -1)
        {
            Timeout = timeout;
            CountTicks = 0;
            MaxTicks = maxTicks;
            _enabled = true;
        }

        public void Stop()
        {
            _enabled = false;
            Complated?.Invoke(CountTicks);
        }


        public void Update()
        {
            if (_enabled)
            {
                if (_time >= Timeout)
                {
                    CountTicks++;
                    _time = 0;
                    Tick?.Invoke(CountTicks);

                    if (MaxTicks != -1 && CountTicks >= MaxTicks)
                    {
                        Stop();
                    }
                }

                _time++;
            }
        }
    }
}