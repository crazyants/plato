using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Reactions.Services
{
    public interface IEntityReactionsManager<TReaction> where TReaction : class
    {
        Task<IActivityResult<TReaction>> CreateAsync(TReaction model);

        Task<IActivityResult<TReaction>> UpdateAsync(TReaction model);

    }

}
