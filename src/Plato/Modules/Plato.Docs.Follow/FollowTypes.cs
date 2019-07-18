using System.Collections.Generic;
using Plato.Follows.Models;
using Plato.Follows.Services;

namespace Plato.Docs.Follow
{

    public class FollowTypes : IFollowTypeProvider
    {

        public static readonly FollowType Doc =
            new FollowType(
                "Doc",
                "Follow Doc",
                "Follow this doc to get notified when major changes or comments are posted",
                "Unsubscribe",
                "You are following this doc and will be notified when major changes & comments are posted, click to unsubscribe",
                "Login to follow this doc",
                "You don't have permission to follow docs");

        public IEnumerable<IFollowType> GetFollowTypes()
        {
            return new[]
            {
                Doc
            };
        }

    }
}
