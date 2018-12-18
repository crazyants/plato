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

  
        private readonly ISafeTimer _safeTimer;
        private readonly ILogger<SafeTimerFactory> _logger;

        //private readonly IServiceProvider _serviceProvider;
        //private readonly IServiceCollection _applicationServices;

        public SafeTimerFactory(
            ISafeTimer safeTimer,
            ILogger<SafeTimerFactory> logger,
            IServiceProvider serviceProvider,
            IServiceCollection applicationServices)
        {
            _safeTimer = safeTimer;
            _logger = logger;
            //_serviceProvider = serviceProvider;
            //_applicationServices = applicationServices;
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


            //// Clone services
            //var tenantServiceCollection = _serviceProvider.CreateChildContainer(_applicationServices);
            //var serviceProvider = tenantServiceCollection.BuildServiceProvider();

            // wrap our action delegate in an awaitable func
            _safeTimer.Elapsed += (sender, args) => action(this, new SafeTimerEventArgs());
            _safeTimer.Options = options;
            _safeTimer.Start();

        }

        public void Stop()
        {
            _safeTimer.Stop();
        }
    }
}
