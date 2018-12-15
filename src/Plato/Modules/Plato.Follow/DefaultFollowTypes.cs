using System.Collections.Generic;
using Plato.Follow.Models;
using Plato.Follow.Services;

namespace Plato.Follow
{
    public class DefaultFollowTypes : IFollowTypeProvider
    {

        public static readonly FollowType Topic = 
            new FollowType("Topic",
                "Follow Topic", 
                "Folow this topic to get notified when replies are posted...",
                "Unsubscribe",
                "You are already following this topic. Unsubscribe below...");
        
        public static readonly FollowType Channel =
            new FollowType("Topic",
                "Follow Channel",
                "Folow this channel to get notified when new topics are posted to this channel...",
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
                Topic,
                Album,
                Article
            };
        }

    }

}
