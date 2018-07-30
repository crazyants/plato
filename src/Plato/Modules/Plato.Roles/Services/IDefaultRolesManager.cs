using System.Threading.Tasks;

namespace Plato.Roles.Services
{
    public interface IDefaultRolesManager
    {
        Task InstallDefaultRolesAsync();

        Task UninstallDefaultRolesAsync();

    }


}
