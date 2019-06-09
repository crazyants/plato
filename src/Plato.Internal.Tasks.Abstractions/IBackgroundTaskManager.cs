using System;

namespace Plato.Internal.Tasks.Abstractions
{

    public interface IBackgroundTaskManager
    {
        
        void StartTasks();

        void StopTasks();

    }

}
