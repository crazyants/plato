using System;

namespace Plato.Internal.Tasks.Abstractions
{
    public interface ISafeTimerFactory
    {

        void Start(Action<object, SafeTimerEventArgs> action, SafeTimerOptions options);

        void Stop();

    }

}
