using System;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Internal.Tasks
{
    
    public class SafeTimerFactory : ISafeTimerFactory
    {

        private readonly ISafeTimer _safeTimer;

        public SafeTimerFactory(ISafeTimer safeTimer)
        {
            _safeTimer = safeTimer;
        }
        
        public void Start(Action<object, SafeTimerEventArgs> action, SafeTimerOptions options)
        {

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _safeTimer.Options = options;
            _safeTimer.Elapsed += (sender, args) => { action(sender, args); };
            _safeTimer.Start();
        }

        public void Stop()
        {
            _safeTimer.Stop();
        }
    }
}
