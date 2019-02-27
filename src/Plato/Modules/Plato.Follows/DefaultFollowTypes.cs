using System.Collections.Generic;
using Plato.Follows.Models;
using Plato.Follows.Services;

namespace Plato.Follows
{
    public class DefaultFollowTypes : IFollowTypeProvider
    {

        public static readonly FollowType Channel =
            new FollowType("Topic",
                "Follow Channel",
                "Follow this channel to get notified when new topics are posted to this channel...",
                "Unsubscribe",
                "You are already following this channel. Unsubscribe below...");
   
        public static readonly FollowType Album = 
            new FollowType("Album");

        public static readonly FollowType Article =
            new FollowType("Article");

        public IEnumerable<IFollowType> GetFollowTypes()
        {
            return new[]
            {
                Album,
                Article
            };
        }

    }

}
