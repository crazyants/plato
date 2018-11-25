using System;

namespace Plato.Internal.Tasks.Abstractions
{
    public interface ISafeTimerFactory
    {

        void Start(Action<object, SafeTimerEventArgs> action, int interval);

        void Stop();

    }

}
