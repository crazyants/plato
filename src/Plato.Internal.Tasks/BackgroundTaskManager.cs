using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Internal.Tasks
{
    
    public class BackgroundTaskManager : IBackgroundTaskManager
    {

       /// <summary>
       /// Globally enable or disable background tasks. Should remain enabled or true unless debugging.
       /// </summary>    
        public bool Enabled { get; set; }= true;
                
        private readonly IEnumerable<IBackgroundTaskProvider> _providers;
        private readonly ILogger<BackgroundTaskManager> _logger;
        private readonly ISafeTimerFactory _safeTimerFactory;

        public BackgroundTaskManager(
            IEnumerable<IBackgroundTaskProvider> providers,
            ILogger<BackgroundTaskManager> logger,
            ISafeTimerFactory safeTimerFactory)
        {
            _safeTimerFactory = safeTimerFactory;
            _providers = providers;
            _logger = logger;
        }

        public void StartTasks()
        {

            if (!Enabled)
            {
                return;
            }

            foreach (var provider in _providers)
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Initializing background task provider of type '{provider.GetType()}'.");
                }
                
                _safeTimerFactory.Start(async (sender, args) =>
                {
                    try
                    {
                        await provider.ExecuteAsync(sender, args);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e,
                            $"An error occurred whilst executing the timer callback for background task provider of type '{provider.GetType()}'");
                        }
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
