using System.Collections.Generic;
using Plato.Entities.Reactions.Models;
using Plato.Entities.Reactions.Services;

namespace Plato.Entities.Reactions
{

    public static class DefaultReactions 
    {

        public static readonly Reaction ThumbUp =
            new Reaction("Thumb Up", "Thumb Up", "👍", 1, Sentiment.Positive);

        public static readonly Reaction ThumbDown  =
            new Reaction("Thumb Down", "Thumb Down", "👎", -1, Sentiment.Negative);

        public static IEnumerable<Reaction> GetReactions()
        {

            return new[]
            {
                ThumbUp,
                ThumbDown
            };

        }

    }

}
