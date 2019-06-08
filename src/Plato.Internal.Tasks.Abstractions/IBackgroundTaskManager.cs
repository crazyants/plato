using System;

namespace Plato.Internal.Tasks.Abstractions
{

    public interface IBackgroundTaskManager
    {
        
        void StartTasks(IServiceProvider serviceProvider);

        void StopTasks();

    }

}
