using Plato.Internal.Models.Shell;

namespace Plato.Internal.Shell.Abstractions
{
    public interface IShellContextFactory
    {
 
        ShellContext CreateShellContext(ShellSettings settings);

        ShellContext CreateSetupContext(ShellSettings settings);

        
    }

}


