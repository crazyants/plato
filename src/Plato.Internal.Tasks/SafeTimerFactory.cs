using System;
using Microsoft.Extensions.Logging;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Internal.Tasks
{
    
    public class SafeTimerFactory : ISafeTimerFactory
    {

        private readonly ISafeTimer _safeTimer;
        private readonly ILogger<SafeTimerFactory> _logger;

        public SafeTimerFactory(
            ISafeTimer safeTimer,
            ILogger<SafeTimerFactory> logger)
        {
            _safeTimer = safeTimer;
            _logger = logger;
        }
        
        public void Start(Action<object, SafeTimerEventArgs> action, SafeTimerOptions options)
        {
         
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogCritical($"Starting timer for action '{action.GetType()}'. Interval: {options.IntervalInSeconds}, RunOnce: {options.RunOnce}, RunOnStart: {options.RunOnStart}");
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
