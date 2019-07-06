using System.Collections.Generic;
using Plato.Follows.Models;
using Plato.Follows.Services;

namespace Plato.Discuss.Follow
{

    public class FollowTypes : IFollowTypeProvider
    {

        public static readonly FollowType Topic =
            new FollowType(
                "Topic",
                "Follow Topic",
                "Follow this topic to get notified when replies are posted",
                "Unsubscribe",
                "You are following this topic and will be notified when replies are posted, click to unsubscribe",
                "Login to follow this topic",
                "You don't have permission to follow topics");

        public IEnumerable<IFollowType> GetFollowTypes()
        {
            return new[]
            {
                Topic
            };
        }

    }
}
