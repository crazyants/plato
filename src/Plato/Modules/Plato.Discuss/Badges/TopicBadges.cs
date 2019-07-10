using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Discuss.Badges
{
    public class TopicBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("TopicBadgesFirst", 
                "First Topic",
                "Started a new topic",
                "fal fa-comment-alt-plus",
                BadgeLevel.Bronze,
                1,
                0);

        public static readonly Badge Bronze =
            new Badge("TopicBadgesBronze",
                "Conversation Starter",
                "Started several topics",
                "fal fa-asterisk",
                BadgeLevel.Bronze,
                10,
                5);

        public static readonly Badge Silver =
            new Badge("TopicBadgesSilver",
                "Converse",
                "Started several topics",
                "fal fa-broadcast-tower",
                BadgeLevel.Silver,
                25,
                10);

        public static readonly Badge Gold =
            new Badge("TopicBadgesGold",
                "Conversationalist",
                "Contributed many topics",
                "fal fa-bullhorn",
                BadgeLevel.Gold,
                50,
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