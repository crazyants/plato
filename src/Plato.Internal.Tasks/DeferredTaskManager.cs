using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Internal.Tasks
{
    
    public class DeferredTaskManager : IDeferredTaskManager
    {
 
        private readonly ILogger<DeferredTaskManager> _logger;
        private readonly IDeferredTaskState _deferredTaskState;

        public DeferredTaskManager(
            IDeferredTaskState deferredTaskState,
       
            ILogger<DeferredTaskManager> logger){
            _deferredTaskState = deferredTaskState;
            _logger = logger;
        }

        public bool HasPendingTasks => _deferredTaskState.Tasks.Any();

        public void AddTask(Func<DeferredTaskContext, Task> task)
        {
            _deferredTaskState.Tasks.Add(task);
        }

        public void ExecuteAsync(Func<DeferredTaskContext, Task> task)
        {

            return;

            //if (_context == null)
            //{
            //    _context  = new DeferredTaskContext(_serviceProvider);
            //}

            //lock (_safeTimerFactory)
            //{
            //    _safeTimerFactory.Start(async (sender, args) =>
            //    {
            //        try
            //        {
            //            await task.Invoke(_context);
            //        }
            //        catch (Exception e)
            //        {
            //            if (_logger.IsEnabled(LogLevel.Critical))
            //            {
            //                _logger.LogError(
            //                    $"An error occurred whilst executing a deferred task. Error: {e.Message}");
            //            }
            //        }
            //    }, new SafeTimerOptions()
            //    {
            //        RunOnStart = true,
            //        RunOnce = true
            //    });
            //}
         
        }

        public async Task ExecuteTaskAsync(DeferredTaskContext context)
        {
            foreach (var task in _deferredTaskState.Tasks)
            {
                try
                {
                    await task(context);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occured while processing a deferred task");
                }
            }

            _deferredTaskState.Tasks.Clear();
        }

    }
    
}
