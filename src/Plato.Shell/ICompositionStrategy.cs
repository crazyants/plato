using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Shell.Models;

namespace Plato.Shell
{
    public interface ICompositionStrategy
    {
        Task<ShellBlueprint> ComposeAsync(ShellSettings settings, ShellDescriptor descriptor);
    }
}
