using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Reactions.Models;

namespace Plato.Reactions.Services
{
    public interface IEntityReactionsManager
    {
        Task<IActivityResult<EntityReaction>> CreateAsync(EntityReaction model);

        Task<IActivityResult<EntityReaction>> UpdateAsync(EntityReaction model);

    }

}
