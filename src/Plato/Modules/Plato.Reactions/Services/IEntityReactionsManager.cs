using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Reactions.Models;

namespace Plato.Reactions.Services
{
    public interface IEntityReactionsManager
    {
        Task<IActivityResult<EntityReacttion>> CreateAsync(EntityReacttion model);

        Task<IActivityResult<EntityReacttion>> UpdateAsync(EntityReacttion model);

    }

}
