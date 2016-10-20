using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Shell.Models;

namespace Plato.Shell
{
    public interface IShellContextFactory
    {
 
        ShellContext CreateShellContext(ShellSettings settings);

        ShellContext CreateSetupContext(ShellSettings settings);

        
    }

}


