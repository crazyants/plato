using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Abstractions.Shell
{
    public enum TenantState
    {
        Uninitialized,
        Initializing,
        Running,
        Disabled,
        Invalid
    }
}
