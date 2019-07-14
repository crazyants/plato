using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Docs.Tags.Badges
{
    public class TagBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("DocsTagBadgesFirst",
                "First Doc Tag",
                "Added a doc tag",
                "fal fa-tag",
                BadgeLevel.Bronze,
                1);
        
        public static readonly Badge Bronze =
            new Badge("DocsTagBadgesBronze",
                "Doc Tagger",
                "Added {threshold} doc tags",
                "fal fa-tag",
                BadgeLevel.Bronze,
                10,
                2);

        public static readonly Badge Silver =
            new Badge("DocsTagBadgesSilver",
                "Doc Tag Artist",
                "Added {threshold} doc tags",
                "fal fa-tag",
                BadgeLevel.Silver,
                25,
                10);

        public static readonly Badge Gold =
            new Badge("DocsArticlesTagBadgesGold",
                "Doc Taxonomist",
                "Added {threshold} doc tags",
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