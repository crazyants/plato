using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Internal.Tasks
{
    
  
    public class SafeTimer : SafeTimerBase, ISafeTimer
    {

        public event TimerEventHandler Elapsed;

        public bool RunOnStart { get; set; }
        
        private Timer _timer;
        private int _inTimerCallback = 0;
        
        public SafeTimer()
        {
            
        }

        public override void Start()
        {

            _timer = new Timer(TimerCallBack, null, Timeout.Infinite, Timeout.Infinite);

            if (IntervalInSeconds <= 0)
                throw new Exception("IntervalInSeconds should be set before starting the timer!");

            base.Start();
            
            _timer.Change(RunOnStart ? 0 : IntervalInSeconds, Timeout.Infinite);
        }

        public override void Stop()
        {
            lock (_timer)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);

                var timer = _timer;
                if (timer != null &&
                    Interlocked.CompareExchange(ref _timer, null, timer) == timer)
                {
                    timer.Dispose();
                }
            }

            base.Stop();

        }

        public override void WaitToStop()
        {
            lock (_timer)
            {
                while (base.PerformingTasks)
                {
                    Monitor.Wait(_timer);
                }
            }

            base.WaitToStop();
        }

        void TimerCallBack(object state)
        {

            if (Interlocked.Exchange(ref _inTimerCallback, 1) != 0)
                return;

            lock (_timer)
            {
                if (!base.IsRunning || base.PerformingTasks)
                    return;

                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                base.PerformingTasks = true;
            }

            try
            {
                if (Elapsed != null)
                {
                    SafeTimerEventArgs e = null;
                    e = state != null ?
                        new SafeTimerEventArgs(state) :
                        new SafeTimerEventArgs();
                    Elapsed(this, e);
                }

            }
            catch
            {

            }
            finally
            {
                lock (_timer)
                {
                    base.PerformingTasks = false;
                    if (base.IsRunning)
                    {
                        _timer.Change(IntervalInSeconds, Timeout.Infinite);
                    }
                    Monitor.Pulse(_timer);
                    Interlocked.Exchange(ref _inTimerCallback, 0);
                }

            }
        }

    }
}
