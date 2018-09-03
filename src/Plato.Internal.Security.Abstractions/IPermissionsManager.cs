using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Security.Abstractions
{
    
    public interface IPermissionsManager<TPermission> where TPermission : class
    {

        IEnumerable<TPermission> GetPermissions();

        Task<IDictionary<string, IEnumerable<TPermission>>> GetCategorizedPermissionsAsync();
        
    }

}
