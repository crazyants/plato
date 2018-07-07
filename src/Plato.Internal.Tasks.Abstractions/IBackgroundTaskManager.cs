using System;

namespace Plato.Internal.Tasks.Abstractions
{
    public interface IBackgroundTaskManager
    {

        void Start(Action<object, SafeTimerEventArgs> action, int interval);

        void Stop();

    }

}
