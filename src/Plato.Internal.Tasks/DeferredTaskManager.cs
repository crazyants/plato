using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Internal.Tasks
{
    
    public class DeferredTaskManager : IDeferredTaskManager
    {
        private DeferredTaskContext _context;

        private readonly ISafeTimerFactory _safeTimerFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DeferredTaskManager> _logger;

        public DeferredTaskManager(
            ISafeTimerFactory safeTimerFactory,
            IServiceProvider serviceProvider, 
            ILogger<DeferredTaskManager> logger)
        {
            _safeTimerFactory = safeTimerFactory;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public void ExecuteAsync(Func<DeferredTaskContext, Task> task)
        {

            if (_context == null)
            {
                _context  = new DeferredTaskContext(_serviceProvider);
            }

            _safeTimerFactory.Start(async (sender, args) =>
            {
                try
                {
                    await task.Invoke(_context);
                }
                catch (Exception e)
                {
                    if (_logger.IsEnabled(LogLevel.Critical))
                    {
                        _logger.LogError(
                            $"An error occurred whilst executing a deferred task. Error: {e.Message}");
                    }
                }
            }, new SafeTimerOptions()
            {
                RunOnStart = true,
                RunOnce = true
            });


        }

    }
    
}
