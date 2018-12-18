using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Internal.Tasks
{

    public class SafeTimerFactory : ISafeTimerFactory
    {
        
        private readonly ILogger<SafeTimerFactory> _logger;

        public SafeTimerFactory(ILogger<SafeTimerFactory> logger)
        {
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
                _logger.LogCritical($"Starting new timer. Interval: {options.IntervalInSeconds}, RunOnce: {options.RunOnce}, RunOnStart: {options.RunOnStart}");
            }

            var safeTimer = new SafeTimer(_logger);
            safeTimer.Elapsed += (sender, args) => action(this, new SafeTimerEventArgs());
            safeTimer.Options = options;
            safeTimer.Start();

        }

        public void Stop()
        {
            //safeTimer.Stop();
        }
    }
}
