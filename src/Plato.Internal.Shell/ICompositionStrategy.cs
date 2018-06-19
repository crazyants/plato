using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Models.Shell;

namespace Plato.Internal.Shell
{
    public interface ICompositionStrategy
    {
        Task<ShellBlueprint> ComposeAsync(IShellSettings settings, IShellDescriptor descriptor);
    }
}
