using System;
using System.Threading.Tasks;

namespace Plato.Internal.Tasks.Abstractions
{

    public interface IDeferredTaskManager
    {
        Task ExecuteAsync(Func<DeferredTaskContext, Task> task);
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
