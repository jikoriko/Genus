using System;
using System.Timers;

namespace Genus2D.Utilities
{
    public class Clock
    {
        private bool _isRunning;
        private Timer _timer;
        private float _timerInterval;
        private int _hours, _minutes, _seconds;

        // Constructor that creates clock at 00:00:00 without starting it
        public Clock()
            : this(0, 0, 0)
        {
        }
        
        // Constructor that creates clock at given time without starting it
        public Clock(int hours, int minutes, int seconds) :
            this(hours, minutes, seconds, false)
        {
        }

        public Clock(int hours, int minutes, int seconds, bool isPlaying)
        {
            _isRunning = isPlaying;
            _hours = hours;
            _minutes = minutes;
            _seconds = seconds;
            _timer = new Timer();
            // set interval to 1 sec
            _timerInterval = 1000;
            _timer.Interval = _timerInterval;
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        }

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        public int Hours
        {
            get { return _hours; }
            set { _hours = value; }
        }
        public int Minutes
        {
            get { return _minutes; }
            set { _minutes = value; }
        }
        public int Seconds
        {
            get { return _seconds; }
            set { _seconds = value; }
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (_isRunning)
            {
                _seconds++;
                if (_seconds == 60)
                {
                    _seconds = 0;
                    _minutes++;
                    if (_minutes == 60)
                    {
                        _minutes = 0;
                        _hours++;
                        if (_hours == 24)
                            _hours = 0;
                    }
                }
            }
        }

        public void IncreaseTimerInterval() //slow down
        {
            if ((float)(1000 / _timerInterval) > 0.25f)
            {
                _timerInterval *= 2;
                _timer.Interval = _timerInterval;
            }
        }

        public void DecreaseTimerInterval() // speed up
        {
            if ((float)(1000 / _timerInterval) < 16.0f)
            {
                _timerInterval /= 2;
                _timer.Interval = _timerInterval;
            }
        }

        public float GetSpeed()
        {
            if (_isRunning)
            {
                return (float)(1000 / _timerInterval);
            }
            else
            {
                return 0;
            }
        }

        public void Start()
        {
            _isRunning = true;
            // reset interval to 1 sec when paused
            _timerInterval = 1000;
            _timer.Interval = _timerInterval;
            _timer.Start();
        }

        public void Pause()
        {
            _isRunning = false;
            _timer.Stop();
        }

        public void Stop()
        {
            // reset to 00:00:00 and remove all entities from map?
        }
    }
}
