using System.Collections.Generic;
using Plato.Reactions.Models;
using Plato.Reactions.Services;

namespace Plato.Reactions.Providers
{
    public class DefaultReactions : IReactionsProvider<Reaction>
    {

        public static readonly Reaction ThumbUp =
            new Reaction("+1", "Thumb Up", "👍", 5);

        public static readonly Reaction ThumbDown  =
            new Reaction("-1", "Thumb Down", "👎", 5);

        public IEnumerable<Reaction> GetReactions()
        {
            return new[]
            {
                ThumbUp,
                ThumbDown
            };
        }
    }
}
