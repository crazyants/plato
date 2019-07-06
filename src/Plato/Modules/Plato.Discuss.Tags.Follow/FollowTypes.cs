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
                "Follow this tag to get notified when new topics or replies are posted with this tag",
                "Unsubscribe",
                "You are following this tag and will be notified when topics or replies are posted with this tag, click to unsubscribe");

        public IEnumerable<IFollowType> GetFollowTypes()
        {
            return new[]
            {
                Tag
            };
        }

    }
}
