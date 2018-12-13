using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Internal.Tasks
{
    
    public class SafeTimer : SafeTimerBase, ISafeTimer
    {
       
        public SafeTimerOptions Options { get; set; } = new SafeTimerOptions();

        public event TimerEventHandler Elapsed;
        
        private Timer _timer;
        private int _inTimerCallback = 0;

        private readonly ILogger<SafeTimer> _logger;

        public SafeTimer(ILogger<SafeTimer> logger)
        {
            _logger = logger;
            _timer = new Timer(TimerCallBack, null, Timeout.Infinite, Timeout.Infinite);
        }

        public override void Start()
        {
            
            if (Options.IntervalInSeconds <= 0 && !Options.RunOnce)
            {
                throw new Exception("IntervalInSeconds should be set before starting the timer!");
            }
                
            base.Start();

            // dueTime
            // The amount of time to delay before invoking
            // the callback method specified when the Timer was
            // constructed, in milliseconds. Specify Infinite to
            // prevent the timer from restarting. Specify zero (0)
            // to restart the timer immediately.
            var dueTime = Options.RunOnStart ? 0 : Options.IntervalInSeconds * 1000;
            dueTime = Options.RunOnce ? Timeout.Infinite : dueTime;

            lock (_timer)
            {
                _timer.Change(dueTime, Timeout.Infinite);
            }

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogCritical($"Started safe timer with dueTime of '{dueTime}'.");
            }
            
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
            {
                return;
            }
                
            lock (_timer)
            {
                if (!base.IsRunning || base.PerformingTasks)
                {
                    return;
                }
                    
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                base.PerformingTasks = true;
            }

            try
            {

                if (Elapsed != null)
                {

                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogCritical($"Executing Elapsed delegate within timer callback on thread: {Thread.CurrentThread.ManagedThreadId}");
                    }
                    
                    Elapsed(this, state != null ?
                        new SafeTimerEventArgs(state) :
                        new SafeTimerEventArgs());

                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogCritical($"Completed executing Elapsed delegate within timer callback on thread: {Thread.CurrentThread.ManagedThreadId}.");
                    }

                }

            }
            catch (Exception e)
            {
                if (_logger.IsEnabled(LogLevel.Critical))
                {
                    _logger.LogCritical($"An error corrected within a timer callback. Error message: {e.Message}");
                }
            }
            finally
            {
                lock (_timer)
                {
                    base.PerformingTasks = false;
                    if (base.IsRunning)
                    {
                        _timer.Change(Options.IntervalInSeconds * 1000, Timeout.Infinite);
                        Monitor.Pulse(_timer);
                        Interlocked.Exchange(ref _inTimerCallback, 0);
                    }
             
                }

            }
        }

    }
}
