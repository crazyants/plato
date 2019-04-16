using System.Collections.Generic;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Questions
{

    public class Reputations : IReputationsProvider<Reputation>
    {

        public static readonly Reputation NewQuestion =
            new Reputation("New Question", 1);

        public static readonly Reputation NewAnswer =
            new Reputation("New Answer", 1);

        public IEnumerable<Reputation> GetReputations()
        {
            return new[]
            {
                NewQuestion,
                NewAnswer
            };
        }

    }
}
