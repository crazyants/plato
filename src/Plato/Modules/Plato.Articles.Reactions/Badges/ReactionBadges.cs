using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Articles.Reactions.Badges
{
    public class ReactionBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge FirstReactor =
            new Badge("Decisive", "Added an article reaction", "fal fa-smile", BadgeLevel.Bronze, 1);
        
        public static readonly Badge BronzeReactor =
            new Badge("Assured", "Added {threshold} article reactions", "fal fa-grin", BadgeLevel.Bronze, 5, 2);

        public static readonly Badge SilverReactor =
            new Badge("Assertive", "Added {threshold} article reactions", "fal fa-smile-wink", BadgeLevel.Silver, 25, 10);

        public static readonly Badge GoldReactor =
            new Badge("Opinionated", "Added {threshold} article reactions", "fal fa-grin-hearts", BadgeLevel.Gold, 50, 25);
        
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