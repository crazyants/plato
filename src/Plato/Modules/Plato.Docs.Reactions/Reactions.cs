using System.Collections.Generic;
using Plato.Entities.Reactions.Models;
using Plato.Entities.Reactions.Services;

namespace Plato.Docs.Reactions
{
    
    public class Reactions : IReactionsProvider<Reaction>
    {
        
        public static readonly Reaction Useful =
            new Reaction("DocsUseful", "Useful +1", "😄", 1, Sentiment.Positive);
   
        public static readonly Reaction Ok =
            new Reaction("DocsOk", "Ok", "😐", 0, Sentiment.Neutral);
  
        public static readonly Reaction Useless =
            new Reaction("DocsUseless", "Useless", "😞", -2, Sentiment.Negative);
        
        public IEnumerable<Reaction> GetReactions()
        {
            return new[]
            {
                Useful,
                Ok,
                Useless
            };
        }

    }

}
