using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Badges;

namespace Plato.Users.Badges.Services
{
    public interface IBadgeEntriesStore
    {

        Task<IEnumerable<IBadgeEntry>> SelectAsync();

        Task<IEnumerable<IBadgeEntry>> SelectByUserIdAsync(int userId);

    }

}
