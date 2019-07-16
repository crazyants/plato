using System.Collections.Generic;
using Plato.Entities.Reactions.Models;
using Plato.Entities.Reactions.Services;

namespace Plato.Articles.Reactions
{
    
    public class Reactions : IReactionsProvider<Reaction>
    {
        
        public static readonly Reaction Useful =
            new Reaction("Useful", "Useful +1", "👍", 1, Sentiment.Positive);
       
        public static readonly Reaction Solved =
            new Reaction("Solved", "Solved +5", "✔️", 5, Sentiment.Positive);

        public static readonly Reaction Ok =
            new Reaction("Ok", "Ok", "👌", 0, Sentiment.Neutral);
        
        public static readonly Reaction Confusing =
            new Reaction("Confusing", "Confusing", "😕", 0, Sentiment.Negative);

        public static readonly Reaction Poor =
            new Reaction("Poor", "Poor", "👎", -1, Sentiment.Negative);

        public static readonly Reaction Useless =
            new Reaction("Useless", "Useless", "😖", -2, Sentiment.Negative);
        
        public IEnumerable<Reaction> GetReactions()
        {
            return new[]
            {
                Useful,
                Solved,
                Ok,
                Confusing,
                Poor,
                Useless,
            };
        }

    }

}
