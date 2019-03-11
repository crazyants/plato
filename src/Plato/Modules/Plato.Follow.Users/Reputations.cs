using System.Collections.Generic;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Follow.Users
{

    public class Reputations : IReputationsProvider<Reputation>
    {

        public static readonly Reputation NewFollow =
            new Reputation("New Follow", 1);

        public static readonly Reputation NewFollower =
            new Reputation("New Follower", 2);

        public IEnumerable<Reputation> GetReputations()
        {
            return new[]
            {
                NewFollow,
                NewFollower
            };
        }

    }
}
