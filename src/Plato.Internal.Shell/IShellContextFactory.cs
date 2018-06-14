using Plato.Internal.Models.Shell;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Internal.Shell
{
    public interface IShellContextFactory
    {
 
        ShellContext CreateShellContext(ShellSettings settings);

        ShellContext CreateSetupContext(ShellSettings settings);

        
    }

}


