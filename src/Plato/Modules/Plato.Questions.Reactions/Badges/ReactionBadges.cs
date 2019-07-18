using System.Collections.Generic;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

namespace Plato.Questions.Reactions.Badges
{
    public class ReactionBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("QuestionReactionBadgesFirst",
                "First Question Reaction",
                "Added a question reaction",
                "fal fa-thumbs-up",
                BadgeLevel.Bronze,
                1);
        
        public static readonly Badge Bronze =
            new Badge("QuestionReactionBadgesBronze",
                "New Question Reactor",
                "Added {threshold} question reactions",
                "fal fa-smile",
                BadgeLevel.Bronze,
                5,
                2);

        public static readonly Badge Silver =
            new Badge("QuestionReactionBadgesSilver",
                "Question Reactor",
                "Added {threshold} question reactions",
                "fal fa-bullhorn",
                BadgeLevel.Silver,
                25,
                10);

        public static readonly Badge Gold =
            new Badge("QuestionReactionBadgesGold",
                "Chain Question Reactor",
                "Added {threshold} question reactions",
                "fal fa-hands-heart", 
                BadgeLevel.Gold,
                50, 
                25);
        
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