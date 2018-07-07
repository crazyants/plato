using System;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Internal.Tasks
{
    
    public class BackgroundTaskManager : IBackgroundTaskManager
    {

        private readonly ISafeTimer _safeTimer;

        public BackgroundTaskManager(ISafeTimer safeTimer)
        {
            _safeTimer = safeTimer;
        }
        
        public void Start(Action<object, SafeTimerEventArgs> action, int interval)
        {
            _safeTimer.IntervalInSeconds = interval;
            _safeTimer.Elapsed += (sender, args) => { action(sender, args); };
            _safeTimer.Start();
        }

        public void Stop()
        {
            _safeTimer.Stop();
        }
    }
}
