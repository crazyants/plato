using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Categories.Services
{

    public interface ICategoryManager<TCategory> where TCategory : class
    {
        Task<IActivityResult<TCategory>> CreateAsync(TCategory model);

        Task<IActivityResult<TCategory>> UpdateAsync(TCategory model);

        Task<IActivityResult<TCategory>> DeleteAsync(TCategory model);

        Task<IActivityResult<TCategory>> AddToRoleAsync(TCategory model, string roleName);

        Task<IActivityResult<TCategory>> RemoveFromRoleAsync(TCategory model, string roleName);

        Task<bool> IsInRoleAsync(TCategory model, string roleName);
        
        Task<IEnumerable<string>> GetRolesAsync(TCategory model);

    }

}
