using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Tasks.Abstractions
{

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
