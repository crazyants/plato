using System.Collections.Generic;
using Plato.Follows.Models;
using Plato.Follows.Services;

namespace Plato.Ideas.Follow
{

    public class FollowTypes : IFollowTypeProvider
    {

        public static readonly FollowType Idea =
            new FollowType(
                "Idea",
                "Follow Idea",
                "Follow this idea to get notified when new comments are posted",
                "Unsubscribe",
                "You are following this idea and will be notified when comments are posted, click to unsubscribe",
                "Login to follow this idea",
                "You don't have permission to follow ideas");

        public IEnumerable<IFollowType> GetFollowTypes()
        {
            return new[]
            {
                Idea
            };
        }

    }
}
