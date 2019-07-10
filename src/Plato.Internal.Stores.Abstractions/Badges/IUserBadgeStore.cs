using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Badges;

namespace Plato.Internal.Stores.Abstractions.Badges
{
    public interface IUserBadgeStore<TModel> : IStore<TModel> where TModel : class
    {
        //Task<IEnumerable<BadgeEntry>> GetUserBadgesAsync(int userId, IEnumerable<IBadge> badges);

    }

}
