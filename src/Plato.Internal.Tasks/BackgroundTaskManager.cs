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

        public void StartTasks()
        {
            foreach (var provider in _providers)
            {
                _safeTimerFactory.Start(async (sender, args) =>
                {
                    try
                    {
                        await provider.ExecuteAsync();
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Critical))
                        {
                            _logger.LogError(
                                $"An error occurred whilst activating background task of type '{provider.GetType()}'. {e.Message}");
                        }
                    }
                }, new SafeTimerOptions()
                {
                    IntervalInSeconds = provider.IntervalInSeconds * 1000
                });

            }

        }

        public void StopTasks()
        {
            _safeTimerFactory.Stop();
        }

    }

}
