using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Discuss.Tags.Badges
{
    public class TagBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("DiscussTagBadgesFirst",
                "First Topic Tag",
                "Added a topic tag",
                "fal fa-tag",
                BadgeLevel.Bronze,
                1);
        
        public static readonly Badge Bronze =
            new Badge("DiscussTagBadgesBronze",
                "Topic Tagger",
                "Added {threshold} topic tags",
                "fal fa-tag",
                BadgeLevel.Bronze,
                10,
                2);

        public static readonly Badge Silver =
            new Badge("DiscussTagBadgesSilver",
                "Topic Tag Artist",
                "Added {threshold} topic tags",
                "fal fa-tag",
                BadgeLevel.Silver,
                25,
                10);

        public static readonly Badge Gold =
            new Badge("DiscussTagBadgesGold",
                "Topic Taxonomist",
                "Added {threshold} topic tags",
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