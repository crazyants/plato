using System.Collections.Generic;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Questions
{

    public class Reputations : IReputationsProvider<Reputation>
    {

        public static readonly Reputation NewArticle =
            new Reputation("New Article", 1);

        public static readonly Reputation NewComment =
            new Reputation("New Comment", 1);

        public IEnumerable<Reputation> GetReputations()
        {
            return new[]
            {
                NewArticle,
                NewComment
            };
        }

    }
}
