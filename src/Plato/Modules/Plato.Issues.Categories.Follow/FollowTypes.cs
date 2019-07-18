using System.Collections.Generic;
using Plato.Follows.Models;
using Plato.Follows.Services;

namespace Plato.Issues.Categories.Follow
{

    public class FollowTypes : IFollowTypeProvider
    {
        
        public static readonly FollowType Category =
            new FollowType(
                "IssueCategory",
                "Follow Category",
                "Follow this category to get notified when new issues are posted within this category",
                "Unsubscribe",
                "You are following this category and will be notified when new issues are posted, click to unsubscribe",
                "Login to follow this category",
                "You don't have permission to follow this category");

        public static readonly FollowType AllCategories =
            new FollowType(
                "IssueAllCategories",
                "Follow All Categories",
                "Follow all categories to get notified when new issues are posted within any category",
                "Unsubscribe",
                "You are following all categories and will be notified when new issues are posted, click to unsubscribe",
                "Login to follow all categories",
                "You don't have permission to follow all categories");
        
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
