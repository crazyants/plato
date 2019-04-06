using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Docs.Badges
{
    public class DocBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("First Doc", "Posted a new doc", "fal fa-comment-alt-plus", BadgeLevel.Bronze, 1, 0);

        public static readonly Badge Bronze =
            new Badge("Doc Starter", "Posted some docs", "fal fa-asterisk", BadgeLevel.Bronze, 10, 5);

        public static readonly Badge Silver =
            new Badge("Documenter", "Contributed several docs", "fal fa-broadcast-tower", BadgeLevel.Silver, 25, 10);
        
        public static readonly Badge Gold =
            new Badge("Doctator", "Contributed many docs", "fal fa-bullhorn", BadgeLevel.Gold, 50, 20);

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