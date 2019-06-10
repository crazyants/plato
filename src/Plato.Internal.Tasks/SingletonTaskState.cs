using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Internal.Tasks
{
    public class SingletonTaskState : IDeferredTaskState
    {
        public IList<Func<DeferredTaskContext, Task>> Tasks { get; } = new List<Func<DeferredTaskContext, Task>>();

    }
}
