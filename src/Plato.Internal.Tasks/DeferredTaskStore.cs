using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Internal.Tasks
{

    public class DeferredTaskStore : IDeferredTaskStore
    {
        public IList<Func<DeferredTaskContext, Task>> Tasks { get; } = new List<Func<DeferredTaskContext, Task>>();

        private readonly ILogger<DeferredTaskStore> _logger;

        public DeferredTaskStore(ILogger<DeferredTaskStore> logger)
        {
            _logger = logger;
        }

        public bool Process(HttpContext httpContext)
        {

            // No need to process if we don't have any tasks
            if (Tasks.Count == 0)
            {
                return false;
            }

            // Only process after a round trip
            var isGet = httpContext.Request.Method.Equals("get", StringComparison.OrdinalIgnoreCase);
            var is200 = httpContext.Response.StatusCode == 200;
            var isHtml = httpContext.Response.ContentType?.StartsWith("text/html", StringComparison.OrdinalIgnoreCase) ?? false;
            return isGet && is200 && isHtml;

        }

        public void AddTask(Func<DeferredTaskContext, Task> task)
        {


            Tasks.Add(async input => await task(input));
    
            //Tasks.Add(async input => await task.Invoke(input));

        }

        public async Task ExecuteTaskAsync(DeferredTaskContext context)
        {
            foreach (var task in Tasks)
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

            Tasks.Clear();

        }


    }
}
