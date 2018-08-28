using System.Collections.Generic;
using System.Runtime.InteropServices;
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


    public interface IPermissionsManager2<TPermission> where TPermission : class
    {

        IEnumerable<TPermission> GetPermissions();

        Task<IDictionary<string, IEnumerable<TPermission>>> GetCategorizedPermissionsAsync();
        
    }

}
