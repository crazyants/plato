using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Users.Badges.BadgeProviders
{

    public class ReputationBadges : IBadgesProvider<Badge>
    {
        
        public static readonly Badge Reputation100 =
            new Badge("Gaining Respect", "Starting to gain reputation", "fal fa-seedling", BadgeLevel.Bronze, 100, 10);

        public static readonly Badge Reputation500 =
            new Badge("Respected", "Earned a decent amount of reputation", "fal fa-burn", BadgeLevel.Silver, 500, 25);

        public static readonly Badge Reputation1000 =
            new Badge("Highly Respected", "Earned lots of reputation", "fab fa-fly", BadgeLevel.Gold, 1000, 50);

        public IEnumerable<Badge> GetBadges()
        {
            return new[]
            {
                Reputation100,
                Reputation500,
                Reputation1000
            };

        }

    }

}
