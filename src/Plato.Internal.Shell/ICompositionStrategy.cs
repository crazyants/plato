using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Shell.Models;

namespace Plato.Internal.Shell
{
    public interface ICompositionStrategy
    {
        Task<ShellBlueprint> ComposeAsync(ShellSettings settings, ShellDescriptor descriptor);
    }
}
