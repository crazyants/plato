using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Issues.Reactions.Badges
{
    public class ReactionBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("IssueReactionBadgesFirst",
                "First Issue Reaction",
                "Added an issue reaction",
                "fal fa-thumbs-up",
                BadgeLevel.Bronze,
                1);
        
        public static readonly Badge Bronze =
            new Badge("IssueReactionBadgesBronze",
                "New Issue Reactor",
                "Added {threshold} issue reactions",
                "fal fa-smile",
                BadgeLevel.Bronze,
                5,
                2);

        public static readonly Badge Silver =
            new Badge("IssueReactionBadgesSilver",
                "Issue Reactor",
                "Added {threshold} issue reactions",
                "fal fa-bullhorn",
                BadgeLevel.Silver,
                25,
                10);

        public static readonly Badge Gold =
            new Badge("IssueReactionBadgesGold",
                "Chain Issue Reactor",
                "Added {threshold} issue reactions",
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