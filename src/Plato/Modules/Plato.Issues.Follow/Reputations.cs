using System.Collections.Generic;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Issues.Follow
{

    public class Reputations : IReputationsProvider<Reputation>
    {

        public static readonly Reputation NewFollow =
            new Reputation("Idea Follow", 1);
        
        public IEnumerable<Reputation> GetReputations()
        {
            return new[]
            {
                NewFollow
            };
        }

    }
}
