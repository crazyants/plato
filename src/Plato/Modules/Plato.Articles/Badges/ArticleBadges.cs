using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Articles.Badges
{
    public class ArticleBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("ArticleBadgesFirst",
                "First Article",
                "Posted an article",
                "fal fa-pencil",
                BadgeLevel.Bronze,
                1,
                0);

        public static readonly Badge Bronze =
            new Badge("ArticleBadgesBronze",
                "Mentor",
                "Posted several articles",
                "fal fa-pen-nib",
                BadgeLevel.Bronze,
                10, 5);

        public static readonly Badge Silver =
            new Badge("ArticleBadgesSilver",
                "Author",
                "Contributed many articles",
                "fal fa-brush",
                BadgeLevel.Silver,
                25,
                10);

        public static readonly Badge Gold =
            new Badge("ArticleBadgesGold",
                "Novelist",
                "Contributed dozens of articles",
                "fal fa-book",
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