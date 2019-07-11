using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Ideas.Tags.Badges
{
    public class TagBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("IdeasTagBadgesFirst",
                "First Idea Tag",
                "Added a new idea tag",
                "fal fa-tag",
                BadgeLevel.Bronze,
                1);
        
        public static readonly Badge Bronze =
            new Badge("IdeasTagBadgesBronze",
                "Idea Tagger",
                "Added {threshold} idea tags",
                "fal fa-tag",
                BadgeLevel.Bronze,
                10,
                2);

        public static readonly Badge Silver =
            new Badge("IdeasTagBadgesSilver",
                "Idea Tag Artist",
                "Added {threshold} idea tags",
                "fal fa-tag",
                BadgeLevel.Silver, 
                25,
                10);

        public static readonly Badge Gold =
            new Badge("IdeasTagBadgesGold",
                "Idea Taxonomist",
                "Added {threshold} idea tags",
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