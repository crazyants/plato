using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Discuss.Reactions.Badges
{
    public class ReactionBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("DiscussReactionBadgesFirst", 
                "First Topic Reaction",
                "Added a topic reaction",
                "fal fa-thumbs-up",
                BadgeLevel.Bronze,
                1);
        
        public static readonly Badge Bronze =
            new Badge("DiscussReactionBadgesBronze",
                "New Topic Reactor",
                "Added {threshold} topic reactions",
                "fal fa-smile",
                BadgeLevel.Bronze,
                5,
                2);

        public static readonly Badge Silver =
            new Badge("DiscussReactionBadgesSilver",
                "Topic Reactor",
                "Added {threshold} topic reactions",
                "fal fa-bullhorn",
                BadgeLevel.Silver,
                25,
                10);

        public static readonly Badge Gold =
            new Badge("DiscussReactionBadgesGold",
                "Chain Topic Reactor",
                "Added {threshold} topic reactions",
                "fal fa-hands-heart", 
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