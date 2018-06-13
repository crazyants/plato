using Plato.Internal.Shell.Abstractions.Models;

namespace Plato.Internal.Shell
{
    public interface IShellContextFactory
    {
 
        ShellContext CreateShellContext(ShellSettings settings);

        ShellContext CreateSetupContext(ShellSettings settings);

        
    }

}


