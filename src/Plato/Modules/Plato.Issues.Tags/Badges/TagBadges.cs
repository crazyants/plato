using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Issues.Tags.Badges
{
    public class TagBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("IssuesTagBadgesFirst",
                "First Issue Tag",
                "Added a new issue tag",
                "fal fa-tag",
                BadgeLevel.Bronze,
                1);
        
        public static readonly Badge Bronze =
            new Badge("IssuesTagBadgesBronze",
                "Issue Tagger",
                "Added {threshold} issue tags",
                "fal fa-tag",
                BadgeLevel.Bronze,
                10,
                2);

        public static readonly Badge Silver =
            new Badge("IssuesTagBadgesSilver",
                "Issue Tag Artist",
                "Added {threshold} issue tags",
                "fal fa-tag",
                BadgeLevel.Silver,
                25,
                10);

        public static readonly Badge Gold =
            new Badge("IssuesTagBadgesGold",
                "Issue Taxonomist",
                "Added {threshold} issue tags",
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