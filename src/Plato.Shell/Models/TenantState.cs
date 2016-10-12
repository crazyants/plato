using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Shell.Models
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
