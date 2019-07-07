using System.Collections.Generic;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Issues.Star
{

    public class Reputations : IReputationsProvider<Reputation>
    {

        public static readonly Reputation StarIssue =
            new Reputation("Star Issue", 1);

        public static readonly Reputation StarredIssue =
            new Reputation("Starred Issue", 2);
        
        public IEnumerable<Reputation> GetReputations()
        {
            return new[]
            {
                StarIssue,
                StarredIssue
            };
        }

    }

}
