using Plato.Badges.Models;
using Plato.Internal.Abstractions;

namespace Plato.Badges.Services
{

    public interface IBadgesAwarderProvider<TModel> where TModel : class
    {

        ICommandResult<TModel> Award(IBadgeAwarderContext<TModel> context);

    }
    
}
