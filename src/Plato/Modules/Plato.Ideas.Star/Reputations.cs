using System.Collections.Generic;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Ideas.Star
{

    public class Reputations : IReputationsProvider<Reputation>
    {

        public static readonly Reputation StarIdea =
            new Reputation("Star Idea", 1);

        public static readonly Reputation StarredIdea =
            new Reputation("Starred Idea", 2);
        
        public IEnumerable<Reputation> GetReputations()
        {
            return new[]
            {
                StarIdea,
                StarredIdea
            };
        }

    }

}
