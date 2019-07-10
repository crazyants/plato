using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Ideas.Badges
{
    public class IdeaCommentBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("IdeaCommentBadgesFirst",
                "First Idea Comment",
                "Posted an idea comment",
                "fal fa-comment",
                BadgeLevel.Bronze,
                1,
                0);

        public static readonly Badge Bronze =
            new Badge("IdeaCommentBadgesBronze",
                "Idea Commenter",
                "Posted several idea comments",
                "fal fa-comment", 
                BadgeLevel.Bronze,
                25,
                5);

        public static readonly Badge Silver =
            new Badge("IdeaCommentBadgesSilver",
                "Idea Contributor",
                "Contributed to several ideas",
                "fal fa-comment",
                BadgeLevel.Silver,
                50,
                10);
        
        public static readonly Badge Gold =
            new Badge("IdeaCommentBadgesGold",
                "Idea Patron",
                "Contributed to dozens of ideas",
                "far fa-comment",
                BadgeLevel.Gold,
                100,
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