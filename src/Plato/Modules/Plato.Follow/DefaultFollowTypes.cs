using System.Collections.Generic;
using Plato.Follow.Models;
using Plato.Follow.Services;

namespace Plato.Follow
{
    public class DefaultFollowTypes : IFollowTypeProvider
    {

        public static readonly FollowType Topic = 
            new FollowType("Topic",
                "Follow", 
                "Folow this topic to get notified when replies are posted...",
                "Unsubscribe",
                "You are already following this topic. Unsubscribe below...");

        public static readonly FollowType User = 
            new FollowType("User");

        public static readonly FollowType Album = 
            new FollowType("Album");

        public static readonly FollowType Article =
            new FollowType("Article");

        public IEnumerable<IFollowType> GetFollowTypes()
        {
            return new[]
            {
                Topic,
                User,
                Album,
                Article
            };
        }

    }

}
