using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
            ILogger<DeferredTaskManager> logger)
        {
            _deferredTaskState = deferredTaskState;
            _logger = logger;
        }
   
        public bool HasTasks => _deferredTaskState.Tasks.Count > 0;

        public void AddTask(Func<DeferredTaskContext, Task> task)
        {
            _deferredTaskState.Tasks.Add(task);
        }

        public bool Process(HttpContext httpContext)
        {

            // No need to process if we don't have any tasks
            if (!HasTasks)
            {
                return false;
            }

            // Only process after a round trip
            var isGet = httpContext.Request.Method.Equals("get", StringComparison.OrdinalIgnoreCase);
            var isSuccess = httpContext.Response.StatusCode == 200;
            var isHtml = httpContext.Response.ContentType?.StartsWith("text/html", StringComparison.OrdinalIgnoreCase) ?? false;
            return isGet && isSuccess && isHtml;

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
