using System.Collections.Generic;
using Plato.Badges.Models;

namespace Plato.Badges.Services
{
    public interface IBadgesProvider<out TBadge> where TBadge : class, IBadge
    {
        IEnumerable<TBadge> GetBadges();
 
    }

}
