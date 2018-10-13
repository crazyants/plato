using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Badges.Services
{
    public interface IBadgesManager<TBadge> where TBadge : class
    {
        IEnumerable<TBadge> GetBadges();

        Task<IDictionary<string, IEnumerable<TBadge>>> GetCategorizedBadgesAsync();

    }

}
