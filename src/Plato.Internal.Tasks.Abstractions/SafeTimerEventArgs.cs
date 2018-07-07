using System;
using System.Collections.Generic;
using System.Text;

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
