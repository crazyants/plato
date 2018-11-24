using System.Threading.Tasks;
using Plato.Badges.Models;
using Plato.Internal.Abstractions;

namespace Plato.Badges.Services
{

    public interface IBadgesAwarderProvider<TModel> where TModel : class
    {

        int IntervalInSeconds { get; set; }

        Task<ICommandResult<TModel>> Award(IBadgeAwarderContext<TModel> context);

    }
    
}
