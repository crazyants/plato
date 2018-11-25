using System.Collections.Generic;
using Plato.Badges.Models;
using Plato.Badges.Services;

namespace Plato.Discuss.Reactions.Badges
{
    public class ReactionBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge FirstReactor =
            new Badge("First Reaction", "Added a reaction", "fal fa-thumbs-up", BadgeLevel.Bronze, 1);
        
        public static readonly Badge BronzeReactor =
            new Badge("New Reactor", "Added several reactions", "fal fa-smile", BadgeLevel.Bronze, 20, 5);

        public static readonly Badge SilverReactor =
            new Badge("Reactor", "Added several reactions", "fal fa-bullhorn", BadgeLevel.Silver, 50, 20);

        public static readonly Badge GoldReactor =
            new Badge("Chain Reactor", "Added many reactions", "fal fa-hands-heart", BadgeLevel.Gold, 100, 30);
        
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