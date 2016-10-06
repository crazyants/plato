using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Environment.Shell.Models;

namespace Plato.Environment.Shell
{
    public interface IShellContextFactory
    {
        /// <summary>
        /// Builds a shell context given a specific tenant settings structure
        /// </summary>
        ShellContext CreateShellContext(ShellSettings settings);

    }

}


