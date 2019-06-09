using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Plato.Internal.Tasks.Abstractions
{

    public interface IDeferredTaskStore
    {
        IList<Func<DeferredTaskContext, Task>> Tasks { get; }

        bool Process(HttpContext httpContext);

        void AddTask(Func<DeferredTaskContext, Task> task);

        Task ExecuteTaskAsync(DeferredTaskContext context);

    }


    public interface IDeferredTaskState
    {
        IList<Func<DeferredTaskContext, Task>> Tasks { get; }
        
    }

    public interface IDeferredTaskManager
    {

        bool HasPendingTasks { get; }

        void AddTask(Func<DeferredTaskContext, Task> task);
        
        Task ExecuteTaskAsync(DeferredTaskContext context);

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
