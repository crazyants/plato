using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Articles.Tags.Badges
{
    public class TagBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("ArticlesTagBadgesFirst",
                "First Article Tag",
                "Added a article tag",
                "fal fa-tag",
                BadgeLevel.Bronze,
                1);
        
        public static readonly Badge Bronze =
            new Badge("ArticlesTagBadgesBronze",
                "Article Tagger",
                "Added {threshold} article tags",
                "fal fa-tag",
                BadgeLevel.Bronze,
                10,
                2);

        public static readonly Badge Silver =
            new Badge("ArticlesTagBadgesSilver",
                "Article Tag Artist",
                "Added {threshold} article tags",
                "fal fa-tag",
                BadgeLevel.Silver,
                25,
                10);

        public static readonly Badge Gold =
            new Badge("ArticlesTagBadgesGold",
                "Article Taxonomist",
                "Added {threshold} article tags",
                "fal fa-tag",
                BadgeLevel.Gold,
                50, 
                25);
        
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