using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Badges;

namespace Plato.Internal.Repositories.Badges
{

    public interface IBadgeDetailsRepository 
    {
        Task<IEnumerable<BadgeDetails>> SelectAsync();

        Task<IEnumerable<BadgeDetails>> SelectByUserIdAsync(int userId);
    }

}
