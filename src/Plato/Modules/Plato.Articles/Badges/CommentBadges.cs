using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Articles.Badges
{
    public class CommentBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("ArticleCommentBadgesFirst",
                "First Comment",
                "Posted a comment",
                "fal fa-comment",
                BadgeLevel.Bronze,
                1,
                0);

        public static readonly Badge Bronze =
            new Badge("ArticleCommentBadgesBronze",
                "Posted several comments",
                "fal fa-comment",
                BadgeLevel.Bronze,
                25,
                5);

        public static readonly Badge Silver =
            new Badge("ArticleCommentBadgesSilver",
                "Contributor",
                "Contributed to several articles",
                "fal fa-comment",
                BadgeLevel.Silver,
                50,
                10);

        public static readonly Badge Gold =
            new Badge("ArticleCommentBadgesGold",
                "Patron",
                "Contributed to dozens of articles",
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