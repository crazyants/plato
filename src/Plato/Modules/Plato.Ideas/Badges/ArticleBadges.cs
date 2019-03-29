using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Ideas.Badges
{
    public class ArticleBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("First Article", "Posted an article", "fal fa-pencil", BadgeLevel.Bronze, 1, 0);

        public static readonly Badge Bronze =
            new Badge("Mentor", "Posted several articles", "fal fa-pencil", BadgeLevel.Bronze, 10, 5);

        public static readonly Badge Silver =
            new Badge("Author", "Contributed many articles", "fal fa-pencil", BadgeLevel.Silver, 25, 10);
        
        public static readonly Badge Gold =
            new Badge("Novelist", "Contributed dozens of articles", "fal fa-pencil-ruler", BadgeLevel.Gold, 50, 20);

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