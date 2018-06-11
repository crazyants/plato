using Plato.Internal.Shell.Models;

namespace Plato.Internal.Shell
{
    public interface IShellContextFactory
    {
 
        ShellContext CreateShellContext(ShellSettings settings);

        ShellContext CreateSetupContext(ShellSettings settings);

        
    }

}


