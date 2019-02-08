using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Discuss.Reactions.Badges
{
    public class DiscussBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge FirstTopic =
            new Badge("First Topic", "Started a topic", "fal fa-comment", BadgeLevel.Bronze, 0, 5);

        public static readonly Badge FirstReply =
            new Badge("First Reply", "Posted a reply", "fal fa-comments", BadgeLevel.Bronze, 0, 5);

        public IEnumerable<Badge> GetBadges()
        {
            return new[]
            {
                FirstTopic,
                FirstReply
            };

        }
        
    }

}