using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Security.Abstractions
{
    public interface IPermissionsManager
    {

        IEnumerable<Permission> GetPermissions();

        Task<IDictionary<string, IEnumerable<Permission>>> GetCategorizedPermissionsAsync();

    }


}
