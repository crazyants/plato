using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Security.Abstractions
{
    public interface IDefaultRolesManager
    {
        Task InstallDefaultRolesAsync();

        Task UpdateDefaultRolesAsync(IPermissionsProvider<Permission> permission);

        Task UpdateDefaultRolesAsync(IEnumerable<IPermissionsProvider<Permission>> permissions);
        
        Task UninstallDefaultRolesAsync();

    }

}
