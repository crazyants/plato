using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Issues.Badges
{
    public class IssueBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("IssueBadgesFirst",
                "First Issue",
                "Posted an issue",
                "fal fa-bug",
                BadgeLevel.Bronze,
                1,
                0);

        public static readonly Badge Bronze =
            new Badge("IssueBadgesBronze",
                "Keen eye",
                "Posted several issues",
                "fal fa-bug",
                BadgeLevel.Bronze,
                10,
                5);

        public static readonly Badge Silver =
            new Badge("IssueBadgesSilver",
                "Sharp eye",
                "Contributed many issues",
                "fal fa-bug",
                BadgeLevel.Silver,
                25, 10);
        
        public static readonly Badge Gold =
            new Badge("IssueBadgesGold",
                "Hawk eye",
                "Contributed dozens of issues",
                "fal fa-bug",
                BadgeLevel.Gold,
                50,
                20);

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