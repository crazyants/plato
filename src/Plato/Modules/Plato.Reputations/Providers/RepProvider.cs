using System.Collections.Generic;
using Plato.Reputations.Services;
using Plato.Reputations.Models;

namespace Plato.Reputations.Providers
{
    public class RepProvider : IReputationProvider<Reputation>
    {

        public static readonly Reputation VisitReputation =
            new Reputation("Visit", "Reputation awared for each visit", 1);

        public IEnumerable<Reputation> GetReputations()
        {
            return new[]
            {
                VisitReputation
            };
        }
    }
}
