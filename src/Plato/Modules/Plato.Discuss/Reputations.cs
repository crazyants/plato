using System.Collections.Generic;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Discuss
{

    public class Reputations : IReputationsProvider<Reputation>
    {

        public static readonly Reputation NewTopic =
            new Reputation("New Topic", 1);

        public static readonly Reputation NewReply =
            new Reputation("New Reply", 1);

        public IEnumerable<Reputation> GetReputations()
        {
            return new[]
            {
                NewTopic,
                NewReply
            };
        }

    }
}
