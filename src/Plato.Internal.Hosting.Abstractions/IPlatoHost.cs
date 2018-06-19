using Plato.Internal.Models.Shell;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Internal.Hosting.Abstractions
{
    public interface IPlatoHost
    {
        void Initialize();

        ShellContext GetOrCreateShellContext(IShellSettings settings);

        void UpdateShellSettings(IShellSettings settings);

        ShellContext CreateShellContext(IShellSettings settings);

        void RecycleShellContext(IShellSettings settings);

        void DisposeShellContext(IShellSettings settings);

    }
}
