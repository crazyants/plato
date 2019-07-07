using System.Collections.Generic;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Questions.Star
{

    public class Reputations : IReputationsProvider<Reputation>
    {

        public static readonly Reputation StarQuestion =
            new Reputation("Star Question", 1);

        public static readonly Reputation StarredQuestion =
            new Reputation("Starred Question", 2);
        
        public IEnumerable<Reputation> GetReputations()
        {
            return new[]
            {
                StarQuestion,
                StarredQuestion
            };
        }

    }

}
