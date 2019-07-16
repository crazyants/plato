using System.Collections.Generic;
using Plato.Follows.Models;
using Plato.Follows.Services;

namespace Plato.Issues.Follow
{

    public class FollowTypes : IFollowTypeProvider
    {

        public static readonly FollowType Issue =
            new FollowType(
                "Issue",
                "Follow Issue",
                "Follow this issue to get notified when new comments are posted",
                "Unsubscribe",
                "You are following this issue and will be notified when comments are posted, click to unsubscribe",
                "Login to follow this issue",
                "You don't have permission to follow issues");

        public IEnumerable<IFollowType> GetFollowTypes()
        {
            return new[]
            {
                Issue
            };
        }

    }
}
