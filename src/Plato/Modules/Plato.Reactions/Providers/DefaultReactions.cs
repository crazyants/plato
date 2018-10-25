using System.Collections.Generic;
using Plato.Reactions.Models;
using Plato.Reactions.Services;

namespace Plato.Reactions.Providers
{
    public class DefaultReactions : IReactionsProvider<Reaction>
    {

        public static readonly Reaction ThumbUp =
            new Reaction("Thumb Up", "Thumb Up +1", "👍", 1);

        public static readonly Reaction ThumbDown  =
            new Reaction("Thumb Down", "Thumb Down -1", "👎", -1);

        public static readonly Reaction Smile =
            new Reaction("Smile", "Smile", "😄", 0);

        public static readonly Reaction Congratulations =
            new Reaction("Congrats", "Congratulations", "🎉", 0);
        
        public static readonly Reaction Confused =
            new Reaction("Confused", "I'm Confused", "😕", 0);

        public static readonly Reaction Heart =
            new Reaction("Heart", "Heart", "❤️", 0);

        public IEnumerable<Reaction> GetReactions()
        {
            return new[]
            {
                ThumbUp,
                ThumbDown,
                Smile,
                Congratulations,
                Confused,
                Heart
            };
        }
    }
}
