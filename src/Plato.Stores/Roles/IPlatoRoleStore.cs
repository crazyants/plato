using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Abstractions.Stores;
using Plato.Models.Roles;

namespace Plato.Stores.Roles
{
    public interface IPlatoRoleStore : IStore<Role>
    {

        Task<Role> GetByName(string name);

        Task<Role> GetByNormalizedName(string nameNormalized);

    }
}