using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Roles;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Internal.Stores.Abstractions.Roles
{
    public interface IPlatoRoleStore : IStore<Role>
    {
        Task<Role> GetByName(string name);

        Task<IList<Role>> GetRolesByUserId(int userId);

        Task<IEnumerable<Role>> GetRolesAsync();
        
        Task<Role> GetByNormalizedName(string nameNormalized);

        Task<IEnumerable<string>> GetRoleNamesAsync();
    }
}