using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Docs.Badges
{
    public class DocBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("DocBadgesFirst",
                "First Doc",
                "Posted a new doc",
                "fal fa-file",
                BadgeLevel.Bronze,
                1,
                0);

        public static readonly Badge Bronze =
            new Badge("DocBadgesBronze",
                "Doc Starter",
                "Posted some docs",
                "fal fa-file-alt",
                BadgeLevel.Bronze,
                10,
                5);

        public static readonly Badge Silver =
            new Badge("DocBadgesSilver",
                "Documenter",
                "Contributed several docs",
                "fal fa-book",
                BadgeLevel.Silver,
                25,
                10);

        public static readonly Badge Gold =
            new Badge("DocBadgesGold",
                "Doctator",
                "Contributed many docs",
                "fal fa-book-heart",
                BadgeLevel.Gold,
                50,
                20);

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