using System.Collections.Generic;
using Plato.Follow.Models;
using Plato.Follow.Services;

namespace Plato.Discuss.Channels.Follow
{

    public class FollowTypes : IFollowTypeProvider
    {
        
        public static readonly FollowType Channel =
            new FollowType("Channel",
                "Follow Channel",
                "Folow this channel to get notified when new content is posted.",
                "Unsubscribe",
                "You are already following this channel.");

        public IEnumerable<IFollowType> GetFollowTypes()
        {
            return new[]
            {
                Channel
            };
        }

    }
}
