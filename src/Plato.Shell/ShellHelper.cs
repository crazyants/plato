using Plato.Abstractions.Shell;
using Plato.Shell.Models;

namespace Plato.Shell
{
    public static class ShellHelper
    {

        public const string DefaultShellName = "Default";

        public static ShellSettings BuildDefaultUninitializedShell = new ShellSettings
        {
            Name = DefaultShellName,
            State = TenantState.Uninitialized
        };

    }
}
