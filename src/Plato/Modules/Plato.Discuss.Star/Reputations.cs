using System.Collections.Generic;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Discuss.Star
{

    public class Reputations : IReputationsProvider<Reputation>
    {

        public static readonly Reputation StarTopic =
            new Reputation("Star Topic", 1);

        public static readonly Reputation StarredTopic =
            new Reputation("Starred Topic", 2);
        
        public IEnumerable<Reputation> GetReputations()
        {
            return new[]
            {
                StarTopic,
                StarredTopic
            };
        }

    }

}
