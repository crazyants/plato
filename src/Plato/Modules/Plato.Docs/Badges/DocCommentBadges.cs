using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Docs.Badges
{
    public class DocCommentBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("DocCommentBadgesFirst", 
                "Assistant",
                "Commented on a doc",
                "fal fa-reply",
                BadgeLevel.Bronze,
                1,
                0);

        public static readonly Badge Bronze =
            new Badge("DocCommentBadgesBronze", 
                "Helper", 
                "Commented within docs",
                "fal fa-reply-all",
                BadgeLevel.Bronze,
                25,
                5);

        public static readonly Badge Silver =
            new Badge("DocCommentBadgesSilver", 
                "Contributor",
                "Commented on several docs",
                "fal fa-coffee",
                BadgeLevel.Silver,
                50,
                10);
        
        public static readonly Badge Gold =
            new Badge("DocCommentBadgesGold",
                "Advocate",
                "Helped improve many docs",
                "far fa-chess-king",
                BadgeLevel.Gold,
                100,
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