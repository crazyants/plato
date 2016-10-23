using Plato.Shell.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Shell
{
    public interface IShellContainerFactory
    {
        IServiceProvider CreateContainer(ShellSettings settings);
    }
}
