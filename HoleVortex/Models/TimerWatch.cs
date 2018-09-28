using System;

namespace FlatPlant.Extensions
{
    public delegate void TimerEventHandler(int countTick);
    
    public class TimerWatch
    {
        public event TimerEventHandler Tick;
        public event TimerEventHandler Complated;
        public int Timeout { get; set; }
        public int CountTicks { get; set; }
        public int MaxTicks { get; set; }
        
        private bool _enabled;
        private int _time { get; set; }

        public TimerWatch()
        {
            Timeout = 0;
            _enabled = false;
            _time = 0;
            CountTicks = 0;
            MaxTicks = -1;
        }

        public void Start(int timeout, int maxTicks = -1)
        {
            Timeout = timeout;
            CountTicks = 0;
            MaxTicks = maxTicks;
            _enabled = true;
        }

        public void Stop() => _enabled = false;
        

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
                        Complated?.Invoke(CountTicks);
                    }
                }

                _time++;
            }
        }
    }
}