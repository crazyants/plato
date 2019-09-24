using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Docs.Reactions.Badges
{
    public class ReactionBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("DocsReactionBadgesFirst",
                "First Doc Reaction", 
                "Added a doc reaction",
                "fal fa-smile",
                BadgeLevel.Bronze,
                1);

        public static readonly Badge Bronze =
            new Badge("DocsReactionBadgesBronze",
                "New Doc Reactor",
                "Added {threshold} doc reactions",
                "fal fa-grin",
                BadgeLevel.Bronze,
                5,
                2);

        public static readonly Badge Silver =
            new Badge("DocsReactionBadgesSilver",
                "Doc Reactor",
                "Added {threshold} doc reactions",
                "fal fa-smile-wink",
                BadgeLevel.Silver,
                25,
                10);

        public static readonly Badge Gold =
            new Badge("DocsReactionBadgesGold",
                "Chain Doc Reactor",
                "Added {threshold} doc reactions",
                "fal fa-grin-hearts", BadgeLevel.Gold,
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