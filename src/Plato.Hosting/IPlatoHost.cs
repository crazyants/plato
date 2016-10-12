using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Shell.Models;
using Plato.Shell;

namespace Plato.Hosting
{
    public interface IPlatoHost
    {
        void Initialize();
        ShellContext GetOrCreateShellContext(ShellSettings settings);
        void UpdateShellSettings(ShellSettings settings);
        ShellContext CreateShellContext(ShellSettings settings);
    }
}
