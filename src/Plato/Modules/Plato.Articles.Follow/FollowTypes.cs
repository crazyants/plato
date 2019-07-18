using System.Collections.Generic;
using Plato.Follows.Models;
using Plato.Follows.Services;

namespace Plato.Articles.Follow
{

    public class FollowTypes : IFollowTypeProvider
    {

        public static readonly FollowType Article =
            new FollowType(
                "Article",
                "Follow Article",
                "Follow this article to get notified when major changes or comments are posted",
                "Unsubscribe",
                "You are following this article and will be notified when major changes & comments are posted, click to unsubscribe",
                "Login to follow this article",
                "You don't have permission to follow articles");

        public IEnumerable<IFollowType> GetFollowTypes()
        {
            return new[]
            {
                Article
            };
        }

    }
}
