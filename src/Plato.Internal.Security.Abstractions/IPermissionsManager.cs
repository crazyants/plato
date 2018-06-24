using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Roles;

namespace Plato.Internal.Security.Abstractions
{
    public interface IPermissionsManager
    {

        IEnumerable<Permission> GetPermissions();

        Task<IDictionary<string, IEnumerable<Permission>>> GetCategorizedPermissionsAsync();
        
        Task<IEnumerable<string>> GetEnabledRolePermissionsAsync(Role role);

    }


}
