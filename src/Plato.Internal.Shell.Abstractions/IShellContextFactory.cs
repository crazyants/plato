using Plato.Internal.Models.Shell;

namespace Plato.Internal.Shell.Abstractions
{
    public interface IShellContextFactory
    {
 
        ShellContext CreateShellContext(IShellSettings settings);

        ShellContext CreateSetupContext(IShellSettings settings);

        ShellContext CreateDescribedContext(IShellSettings settings, IShellDescriptor descriptor);



    }

}


