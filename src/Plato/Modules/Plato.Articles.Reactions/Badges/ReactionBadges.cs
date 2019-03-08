using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Articles.Reactions.Badges
{
    public class ReactionBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge FirstReactor =
            new Badge("First Reaction", "Added a reaction", "fal fa-thumbs-up", BadgeLevel.Bronze, 1);
        
        public static readonly Badge BronzeReactor =
            new Badge("New Reactor", "Added {threshold} reactions", "fal fa-smile", BadgeLevel.Bronze, 5, 2);

        public static readonly Badge SilverReactor =
            new Badge("Reactor", "Added {threshold} reactions", "fal fa-bullhorn", BadgeLevel.Silver, 25, 10);

        public static readonly Badge GoldReactor =
            new Badge("Chain Reactor", "Added {threshold} reactions", "fal fa-hands-heart", BadgeLevel.Gold, 50, 25);
        
        public IEnumerable<Badge> GetBadges()
        {
            return new[]
            {
                FirstReactor,
                BronzeReactor,
                SilverReactor,
                GoldReactor
            };

        }
        
    }

}