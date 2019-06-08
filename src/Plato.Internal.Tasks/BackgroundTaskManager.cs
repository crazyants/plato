using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Internal.Tasks
{
    
    public class BackgroundTaskManager : IBackgroundTaskManager
    {

        private readonly ISafeTimerFactory _safeTimerFactory;
        private readonly IEnumerable<IBackgroundTaskProvider> _providers;
        private readonly ILogger<BackgroundTaskManager> _logger;
        
        public BackgroundTaskManager(
            IEnumerable<IBackgroundTaskProvider> providers,
            ISafeTimerFactory safeTimerFactory,
            ILogger<BackgroundTaskManager> logger)
        {
            _providers = providers;
            _safeTimerFactory = safeTimerFactory;
            _logger = logger;
        }

        public void StartTasks(IServiceProvider serviceProvider)
        {

            var timerFactory = new SafeTimerFactory(serviceProvider);
            
            foreach (var provider in _providers)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Initializing background task provider of type '{provider.GetType()}'.");
                }


                timerFactory.Start(async (sender, args) =>
                {
                    try
                    {
                        await provider.ExecuteAsync(sender, args);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,
                            $"An error occurred whilst executing the timer callback for background task provider of type '{provider.GetType()}'");
                    }
                }, new SafeTimerOptions()
                {
                    Owner = provider.GetType(),
                    IntervalInSeconds = provider.IntervalInSeconds
                });


            }

        }

        public void StopTasks()
        {
            _safeTimerFactory.Stop();
        }

    }

}
