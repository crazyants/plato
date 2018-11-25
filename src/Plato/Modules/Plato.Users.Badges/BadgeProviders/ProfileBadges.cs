using System.Collections.Generic;
using Plato.Badges.Models;
using Plato.Badges.Services;

namespace Plato.Users.Badges.BadgeProviders
{
    public class ProfileBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge ConfirmedMember =
            new Badge("Confirmed", "I'm legit me", "fal fa-check", BadgeLevel.Bronze, 0, 5);

        public static readonly Badge Autobiographer =
            new Badge("Autobiographer", "Added a profile", "fal fa-user", BadgeLevel.Bronze, 0, 5);

        public IEnumerable<Badge> GetBadges()
        {
            return new[]
            {
                ConfirmedMember,
                Autobiographer
            };

        }
     
    }

}
