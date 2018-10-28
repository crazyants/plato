using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Categories.Services
{

    public interface ICategoryManager<TCategory> : ICommandManager<TCategory> where TCategory : class
    {

        Task<ICommandResult<TCategory>> AddToRoleAsync(TCategory model, string roleName);

        Task<ICommandResult<TCategory>> RemoveFromRoleAsync(TCategory model, string roleName);

        Task<bool> IsInRoleAsync(TCategory model, string roleName);
        
        Task<IEnumerable<string>> GetRolesAsync(TCategory model);
        
        Task<ICommandResult<TCategory>> Move(TCategory model, MoveDirection direction);

    }

    public enum MoveDirection
    {
        Up,
        Down
    }

}
