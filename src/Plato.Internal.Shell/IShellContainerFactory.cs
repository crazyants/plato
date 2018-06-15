using System;
using Plato.Internal.Models.Shell;

namespace Plato.Internal.Shell
{
    public interface IShellContainerFactory
    {
        IServiceProvider CreateContainer(ShellSettings settings, ShellBlueprint blueprint);
        
    }
}
