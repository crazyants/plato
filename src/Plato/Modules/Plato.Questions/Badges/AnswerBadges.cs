using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Questions.Badges
{
    public class AnswerBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("First Comment", "Posted an answer", "fal fa-check-circle", BadgeLevel.Bronze, 1, 0);

        public static readonly Badge Bronze =
            new Badge("Commenter", "Posted several answers", "fal fa-check-circle", BadgeLevel.Bronze, 25, 5);

        public static readonly Badge Silver =
            new Badge("Contributor", "Contributed to many questions", "fal fa-check-circle", BadgeLevel.Silver, 50, 10);
        
        public static readonly Badge Gold =
            new Badge("Patron", "Contributed to dozens of questions", "far fa-check-circle", BadgeLevel.Gold, 100, 20);

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