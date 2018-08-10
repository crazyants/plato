using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Reactions.Models;

namespace Plato.Reactions.Services
{
    public interface IReactionManager
    {
        Task<IActivityResult<Reaction>> CreateAsync(Reaction model);

        Task<IActivityResult<Reaction>> UpdateAsync(Reaction model);

    }

}
