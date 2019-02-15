using System.Threading.Tasks;

namespace Plato.Internal.Security.Abstractions
{
    public interface IDefaultRolesManager
    {
        Task InstallDefaultRolesAsync();

        Task UninstallDefaultRolesAsync();

    }

}
