using System.Collections.Generic;
using Plato.Follows.Models;
using Plato.Follows.Services;

namespace Plato.Discuss.Categories.Follow
{

    public class FollowTypes : IFollowTypeProvider
    {
        
        public static readonly FollowType Category =
            new FollowType(
                "DiscussChannel",
                "Follow Category",
                "Follow this category to get notified when new topics are posted within this category",
                "Unsubscribe",
                "You are already following this category, click to unsubscribe");
        
        public static readonly FollowType AllCategories =
            new FollowType(
                "DiscussAllChannels",
                "Follow All Channels",
                "Follow all categories to get notified when new topics are posted within any category",
                "Unsubscribe",
                "You are already following all categories, click to unsubscribe");
        
        public IEnumerable<IFollowType> GetFollowTypes()
        {
            return new[]
            {
                Category,
                AllCategories
            };
        }

    }

}
