using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Articles.Badges
{
    public class ReplyBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("First Comment", "Posted a new reply", "fal fa-reply", BadgeLevel.Bronze, 1, 0);

        public static readonly Badge Bronze =
            new Badge("Responsive Commenter", "Contributed several comments", "fal fa-reply-all", BadgeLevel.Bronze, 25, 5);

        public static readonly Badge Silver =
            new Badge("Staying Engaged", "Contributed many comments", "fal fa-coffee", BadgeLevel.Silver, 50, 10);
        
        public static readonly Badge Gold =
            new Badge("Philosopher", "Made significant contributions", "far fa-chess-king", BadgeLevel.Gold, 100, 20);

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