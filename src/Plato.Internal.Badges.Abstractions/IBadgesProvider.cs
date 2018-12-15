using System.Collections.Generic;
using Plato.Internal.Models.Badges;

namespace Plato.Internal.Badges.Abstractions
{
    public interface IBadgesProvider<out TBadge> where TBadge : class, IBadge
    {
        IEnumerable<TBadge> GetBadges();
    }

}
