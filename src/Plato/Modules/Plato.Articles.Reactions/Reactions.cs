using System.Collections.Generic;
using Plato.Entities.Reactions.Models;
using Plato.Entities.Reactions.Services;

namespace Plato.Articles.Reactions
{
    
    public class Reactions : IReactionsProvider<Reaction>
    {
        
        public static readonly Reaction Useful =
            new Reaction("ArticlesUseful", "Useful", "👍", 1, Sentiment.Positive);
       
        public static readonly Reaction Solved =
            new Reaction("ArticlesSolved", "Solved", "✔️", 5, Sentiment.Positive);

        public static readonly Reaction Ok =
            new Reaction("ArticlesOk", "Ok", "👌", 0, Sentiment.Neutral);
        
        public static readonly Reaction Confusing =
            new Reaction("ArticlesConfusing", "Confusing", "😕", 0, Sentiment.Negative);

        public static readonly Reaction Poor =
            new Reaction("ArticlesPoor", "Poor", "👎", -1, Sentiment.Negative);

        public static readonly Reaction Useless =
            new Reaction("ArticlesUseless", "Useless", "😖", -2, Sentiment.Negative);
        
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
