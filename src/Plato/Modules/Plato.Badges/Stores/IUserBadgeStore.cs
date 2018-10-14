using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Badges.Models;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Badges.Stores
{
    public interface IUserBadgeStore<TModel> : IStore<TModel> where TModel : class
    {
        Task<IEnumerable<Badge>> GetUserBadgesAsync(int userId);

    }

}
