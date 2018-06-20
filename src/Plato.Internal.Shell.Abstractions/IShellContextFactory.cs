using Plato.Internal.Models.Shell;

namespace Plato.Internal.Shell.Abstractions
{
    public interface IShellContextFactory
    {


        ShellContext CreateMinimalShellContext(IShellSettings settings);

        ShellContext CreateShellContext(IShellSettings settings);

        ShellContext CreateSetupContext(IShellSettings settings);

        ShellContext CreateDescribedContext(IShellSettings settings, IShellDescriptor descriptor);

        ShellDescriptor MinimumShellDescriptor();

    }

}


