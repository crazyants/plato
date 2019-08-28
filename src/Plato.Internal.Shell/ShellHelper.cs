using Plato.Internal.Models.Shell;

namespace Plato.Internal.Shell
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
