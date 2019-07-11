using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Users.Badges
{
    public class VisitBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("VisitBadgesFirst",
                "New Comer",
                "I'm new here",
                "fal fa-at",
                BadgeLevel.Bronze);
        
        public static readonly Badge Bronze =
            new Badge("VisitBadgesBronze",
                "Getting into this",
                "Starting to like this",
                "fal fa-heart",
                BadgeLevel.Bronze,
                5,
                10);

        public static readonly Badge Silver =
            new Badge("VisitBadgesSilver",
                "I'm a regular here",
                "Can't kep away",
                "fal fa-splotch",
                BadgeLevel.Silver, 10,
                20);

        public static readonly Badge Gold =
            new Badge("VisitBadgesGold",
                "I may be obsessed",
                "Here all the time",
                "fal fa-rocket",
                BadgeLevel.Gold,
                20,
                30);
        
        public IEnumerable<Badge> GetBadges()
        {
            return new[]
            {
                First,
                Bronze,
                Silver,
                Gold
            };

        }
        
    }

}