using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Shell.Models;

namespace Plato.Shell
{
    public interface IShellContextFactory
    {
        /// <summary>
        /// Builds a shell context given a specific tenant settings structure
        /// </summary>
        ShellContext CreateShellContext(ShellSettings settings);
        ShellContext CreateSetupContext(ShellSettings settings);

        
    }

}


