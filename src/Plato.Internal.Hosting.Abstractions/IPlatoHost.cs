using Plato.Internal.Models.Shell;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Internal.Hosting.Abstractions
{
    public interface IPlatoHost
    {
        void Initialize();

        ShellContext GetOrCreateShellContext(ShellSettings settings);

        void UpdateShellSettings(ShellSettings settings);

        ShellContext CreateShellContext(ShellSettings settings);

        void RecycleShellContext(ShellSettings settings);

        void DisposeShellContext(ShellSettings settings);

    }
}
