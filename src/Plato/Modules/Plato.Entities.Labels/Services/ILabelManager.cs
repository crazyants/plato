using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Entities.Labels.Services
{

    public interface ILabelManager<TLabel> : ICommandManager<TLabel> where TLabel : class
    {
   
        Task<ICommandResult<TLabel>> AddToRoleAsync(TLabel model, string roleName);

        Task<ICommandResult<TLabel>> RemoveFromRoleAsync(TLabel model, string roleName);

        Task<bool> IsInRoleAsync(TLabel model, string roleName);
        
        Task<IEnumerable<string>> GetRolesAsync(TLabel model);

    }

}
