using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Internal.Tasks
{

    public class SafeTimerFactory : ISafeTimerFactory
    {

        private IList<ISafeTimer> _timers = null;

        private readonly ILogger<SafeTimerFactory> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SafeTimerFactory(
            ILogger<SafeTimerFactory> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Start(Action<object, SafeTimerEventArgs> action, SafeTimerOptions options)
        {

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (_timers == null)
            {
                _timers = new List<ISafeTimer>();
            }
            
            //var tenantServiceCollection = _serviceProvider.CreateChildContainer(_applicationServices);
            //var serviceProvider = tenantServiceCollection.BuildServiceProvider();

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogCritical($"Starting new timer. Interval: {options.IntervalInSeconds}, RunOnce: {options.RunOnce}, RunOnStart: {options.RunOnStart}");
            }

            var safeTimer = new SafeTimer(_httpContextAccessor.HttpContext);
            safeTimer.Elapsed += (sender, args) => action(sender, args);
            safeTimer.Options = options;
            safeTimer.Start();

            _timers.Add(safeTimer);

        }

        public void Stop()
        {

            if (_timers == null)
            {
                return;
            }

            foreach (var timer in _timers)
            {
                timer.WaitToStop();
            }
       
        }
    }
}
