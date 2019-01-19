using System.Collections.Generic;
using Plato.Follows.Models;
using Plato.Follows.Services;

namespace Plato.Discuss.Tags.Follow
{

    public class FollowTypes : IFollowTypeProvider
    {
        
        public static readonly FollowType Tag =
            new FollowType("DiscussTag",
                "Follow Tag",
                "Follow this tag to get notified when new topics are posted with this tag.",
                "Unsubscribe",
                "You are already following this tag. Unsubscribe below...");

        public IEnumerable<IFollowType> GetFollowTypes()
        {
            return new[]
            {
                Tag
            };
        }

    }
}
