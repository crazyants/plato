using System;
using System.Collections.Generic;
using Plato.Badges.Models;
using Plato.Badges.Services;

namespace Plato.Badges
{
    public class Badges : IBadgesProvider<Badge>
    {
        public static readonly Badge BronzeVisitor =
            new Badge("BronzeVisitor", "Visitor I", BadgeLevel.Bronze, 10);

        public static readonly Badge SilverVisitor =
            new Badge("SilverVisitor", "Visitor II", BadgeLevel.Silver, 20);

        public static readonly Badge GoldVisitor =
            new Badge("GoldVisitor", "Visitor III", BadgeLevel.Gold, 30);
        
        public IEnumerable<Badge> GetBadges()
        {
            return new[]
            {
                BronzeVisitor,
                SilverVisitor,
                GoldVisitor
            };

        }

        public IEnumerable<DefaultBadges<Badge>> GetDefaultBadges()
        {
            return new[]
            {
                new DefaultBadges<Badge>
                {
                    Feature = "Plato.Badges",
                    Badges = GetBadges()
                }
            };
        }
        

    }

}