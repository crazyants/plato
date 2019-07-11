using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Discuss.Badges
{
    public class ReplyBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("TopicReplyBadgesFirst", 
                "First Topic Reply",
                "Contributed within a topic",
                "fal fa-reply",
                BadgeLevel.Bronze,
                1,
                0);

        public static readonly Badge Bronze =
            new Badge("TopicReplyBadgesBronze",
                "Responsive",
                "Contributed to several topics",
                "fal fa-reply-all",
                BadgeLevel.Bronze,
                25,
                5);

        public static readonly Badge Silver =
            new Badge("TopicReplyBadgesSilver",
                "Staying Engaged",
                "Contributed to dozens of topics",
                "fal fa-coffee",
                BadgeLevel.Silver,
                50,
                10);
        
        public static readonly Badge Gold =
            new Badge("TopicReplyBadgesGold",
                "Philosopher",
                "Contributed to many topics",
                "far fa-chess-king",
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