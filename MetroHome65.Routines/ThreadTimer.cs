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

        private ThreadTimerProc _timerProc;

        public ThreadTimer(int interval, ThreadTimerProc timerProc,  int startDelay)
        {
            _interval = interval;
            _timerProc = timerProc;
            _active = true;

            _thread = new Thread(() =>
            {
                SafeSleep(startDelay);

                while (_active)
                {
                    if (_timerProc != null)
                        _timerProc();
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
            _timerProc = null;

            if (_thread != null)
            {
                _thread.Join();
                _thread = null;
            }
        }

        public void SafeSleep(int timeoutMs)
        {
            for (var i = 0; i < timeoutMs; i += 100)
            {
                if (!_active) return;
                Thread.Sleep(100);
                //Application.DoEvents();
            }
        }

    }
}
