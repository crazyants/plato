using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Users.Badges.BadgeProviders
{
    public class VisitBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge NewMember =
            new Badge("New Comer", "I'm new here", "fal fa-at", BadgeLevel.Bronze);
        
        public static readonly Badge BronzeVisitor =
            new Badge("Getting into this", "Starting to like this", "fal fa-heart", BadgeLevel.Bronze, 5, 10);

        public static readonly Badge SilverVisitor =
            new Badge("I'm a regular here", "Can't kep away", "fal fa-splotch", BadgeLevel.Silver, 10, 20);

        public static readonly Badge GoldVisitor =
            new Badge("I may be obsessed", "Here all the time", "fal fa-rocket", BadgeLevel.Gold, 20, 30);
        
        public IEnumerable<Badge> GetBadges()
        {
            return new[]
            {
                NewMember,
                BronzeVisitor,
                SilverVisitor,
                GoldVisitor
            };

        }
        
    }

}