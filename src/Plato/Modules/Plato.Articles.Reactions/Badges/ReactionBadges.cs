using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Articles.Reactions.Badges
{
    public class ReactionBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("ArticlesReactionBadgesFirst",
                "First Article Reaction", 
                "Added an article reaction",
                "fal fa-smile",
                BadgeLevel.Bronze,
                1);
        
        public static readonly Badge Bronze =
            new Badge("ArticlesReactionBadgesBronze",
                "New Article Reactor",
                "Added {threshold} article reactions",
                "fal fa-grin",
                BadgeLevel.Bronze,
                5,
                2);

        public static readonly Badge Silver =
            new Badge("ArticlesReactionBadgesSilver",
                "Article Reactor",
                "Added {threshold} article reactions",
                "fal fa-smile-wink",
                BadgeLevel.Silver,
                25, 
                10);

        public static readonly Badge Gold =
            new Badge("ArticlesReactionBadgesGold",
                "Chain Article Reactor",
                "Added {threshold} article reactions",
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