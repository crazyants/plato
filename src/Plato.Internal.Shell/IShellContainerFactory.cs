using System;
using Plato.Internal.Models.Shell;

namespace Plato.Internal.Shell
{
    public interface IShellContainerFactory
    {
        IServiceProvider CreateContainer(IShellSettings settings, ShellBlueprint blueprint);
        
    }
}
