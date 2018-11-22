using System.Threading.Tasks;
using Plato.Badges.Models;
using Plato.Internal.Abstractions;

namespace Plato.Badges.Services
{
    
    public interface IBadgesAwarderProvider<TModel> where TModel : class
    {

        Task<ICommandResult<TModel>> AwardAsync(IBadgeAwarderContext<TModel> context);

    }
    
}
