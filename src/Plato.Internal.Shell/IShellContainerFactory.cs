using Plato.Internal.Shell.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Internal.Shell
{
    public interface IShellContainerFactory
    {
        IServiceProvider CreateContainer(ShellSettings settings);
    }
}
