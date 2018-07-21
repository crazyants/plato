using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Labels.Services
{

    public interface ILabelManager<TLabel> where TLabel : class
    {
        Task<IActivityResult<TLabel>> CreateAsync(TLabel model);

        Task<IActivityResult<TLabel>> UpdateAsync(TLabel model);

        Task<IActivityResult<TLabel>> DeleteAsync(TLabel model);

        Task<IActivityResult<TLabel>> AddToRoleAsync(TLabel model, string roleName);

        Task<IActivityResult<TLabel>> RemoveFromRoleAsync(TLabel model, string roleName);

        Task<bool> IsInRoleAsync(TLabel model, string roleName);
        
        Task<IEnumerable<string>> GetRolesAsync(TLabel model);

    }

}
