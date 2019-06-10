using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Plato.Internal.Tasks.Abstractions
{
    
    public interface IDeferredTaskState
    {
        IList<Func<DeferredTaskContext, Task>> Tasks { get; }
        
    }

    public interface IDeferredTaskManager
    {
        
        bool HasTasks { get; }

        void AddTask(Func<DeferredTaskContext, Task> task);
        
        Task ExecuteTaskAsync(DeferredTaskContext context);

        bool Process(HttpContext httpContext);

    }



    public class DeferredTaskContext
    {
        public DeferredTaskContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }
    }
    
}
