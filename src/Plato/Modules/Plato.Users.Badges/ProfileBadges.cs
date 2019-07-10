using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Users.Badges
{
    public class ProfileBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge ConfirmedMember =
            new Badge("Confirmed",
                "Confirmed",
                "I'm legit me",
                "fal fa-check",
                BadgeLevel.Bronze,
                0,
                10);

        public static readonly Badge Autobiographer =
            new Badge("Autobiographer", 
                "Autobiographer", 
                "Added a biography",
                "fal fa-user",
                BadgeLevel.Bronze,
                0,
                2);

        public static readonly Badge Personalizer =
            new Badge("Personalizer",
                "Personalizer",
                "Added a signature",
                "fal fa-palette",
                BadgeLevel.Bronze,
                0,
                2);

        public IEnumerable<Badge> GetBadges()
        {
            return new[]
            {
                ConfirmedMember,
                Autobiographer,
                Personalizer
            };

        }
     
    }

}
