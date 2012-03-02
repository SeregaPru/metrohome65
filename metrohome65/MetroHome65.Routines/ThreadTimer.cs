using System;
using System.Threading;

namespace MetroHome65.Routines
{
    public delegate void ThreadTimerProc();

    public class ThreadTimer
    {
        /// <summary>
        /// interval in milliseconds
        /// </summary>
        private readonly int _interval;

        private Thread _thread;

        private Boolean _active;

        public ThreadTimer(int interval, ThreadTimerProc timerProc,  int startDelay)
        {
            _interval = interval;
            _active = true;

            _thread = new Thread(() =>
            {
                SafeSleep(startDelay);

                while (_active)
                {
                    timerProc();
                    SafeSleep(_interval);
                }
            });
            _thread.Start();
        }

        public ThreadTimer(int interval, ThreadTimerProc timerProc) : this(interval, timerProc, 0) { }

        ~ThreadTimer()
        {
            Stop();
        }

        public void Stop()
        {
            _active = false;

            if (_thread != null)
            {
                _thread.Join(5000);
                _thread = null;
            }
        }

        public void SafeSleep(int timeoutMs)
        {
            for (var i = 0; i < timeoutMs; i += 100)
            {
                if (!_active) return;
                Thread.Sleep(100);
            }
        }

    }
}
