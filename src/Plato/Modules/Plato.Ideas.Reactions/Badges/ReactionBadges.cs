using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Ideas.Reactions.Badges
{
    public class ReactionBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("IdeaReactionBadgesFirst",
                "First Idea Reaction",
                "Added an idea reaction",
                "fal fa-thumbs-up",
                BadgeLevel.Bronze,
                1);
        
        public static readonly Badge Bronze =
            new Badge("IdeaReactionBadgesBronze",
                "New Idea Reactor",
                "Added {threshold} idea reactions",
                "fal fa-smile",
                BadgeLevel.Bronze,
                5,
                2);

        public static readonly Badge Silver =
            new Badge("IdeaReactionBadgesSilver",
                "Idea Reactor",
                "Added {threshold} idea reactions",
                "fal fa-bullhorn",
                BadgeLevel.Silver,
                25,
                10);

        public static readonly Badge Gold =
            new Badge("IdeaReactionBadgesGold",
                "Chain Idea Reactor",
                "Added {threshold} idea reactions",
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