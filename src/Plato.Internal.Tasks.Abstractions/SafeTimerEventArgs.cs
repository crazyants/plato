using System;

namespace Plato.Internal.Tasks.Abstractions
{
    public class SafeTimerEventArgs : EventArgs
    {

        public object State;

        public SafeTimerEventArgs()
        {
        }

        public SafeTimerEventArgs(object state)
        {
            State = state;
        }

    }
    
}
