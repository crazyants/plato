using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Questions.Badges
{
    public class CommentBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("First Comment", "Posted a comment", "fal fa-comment", BadgeLevel.Bronze, 1, 0);

        public static readonly Badge Bronze =
            new Badge("Commenter", "Posted several comments", "fal fa-comment", BadgeLevel.Bronze, 25, 5);

        public static readonly Badge Silver =
            new Badge("Contributor", "Contributed to several articles", "fal fa-comment", BadgeLevel.Silver, 50, 10);
        
        public static readonly Badge Gold =
            new Badge("Patron", "Contributed to dozens of articles", "far fa-comment", BadgeLevel.Gold, 100, 20);

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