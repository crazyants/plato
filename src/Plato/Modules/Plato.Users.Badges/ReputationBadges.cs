using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Users.Badges
{

    public class ReputationBadges : IBadgesProvider<Badge>
    {
        
        public static readonly Badge Bronze =
            new Badge("ReputationBadgesBronze",
                "Gaining Respect",
                "Starting to gain reputation",
                "fal fa-seedling",
                BadgeLevel.Bronze,
                100,
                10);

        public static readonly Badge Silver =
            new Badge("ReputationBadgesSilver",
                "Respected",
                "Earned a decent amount of reputation",
                "fal fa-burn",
                BadgeLevel.Silver,
                500, 25);

        public static readonly Badge Gold =
            new Badge("ReputationBadgesGold",
                "Highly Respected",
                "Earned lots of reputation",
                "fab fa-fly",
                BadgeLevel.Gold,
                1000,
                50);

        public IEnumerable<Badge> GetBadges()
        {
            return new[]
            {
                Bronze,
                Silver,
                Gold
            };

        }

    }

}
