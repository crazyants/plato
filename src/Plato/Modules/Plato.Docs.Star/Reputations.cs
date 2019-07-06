using System.Collections.Generic;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Docs.Star
{

    public class Reputations : IReputationsProvider<Reputation>
    {

        public static readonly Reputation StarDoc =
            new Reputation("Star Doc", 1);

        public static readonly Reputation StarredDoc =
            new Reputation("Starred Doc", 2);
        
        public IEnumerable<Reputation> GetReputations()
        {
            return new[]
            {
                StarDoc,
                StarredDoc
            };
        }

    }

}
