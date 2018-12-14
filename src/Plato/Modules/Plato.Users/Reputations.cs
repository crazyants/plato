using System.Collections.Generic;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Users
{

    public class Reputations : IReputationsProvider<Reputation>
    {

        public static readonly Reputation UniqueVisit =
            new Reputation("Visit", "Reputation awared for every unique visit", 1);

        public static readonly Reputation NewTopicReputation =
            new Reputation("New Topic", "Reputation awared for posting a new topic.", 1);

        public IEnumerable<Reputation> GetReputations()
        {
            return new[]
            {
                UniqueVisit,
                NewTopicReputation
            };
        }

    }
}
