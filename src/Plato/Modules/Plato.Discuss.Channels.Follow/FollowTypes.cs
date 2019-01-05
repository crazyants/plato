using System.Collections.Generic;
using Plato.Follows.Models;
using Plato.Follows.Services;

namespace Plato.Discuss.Channels.Follow
{

    public class FollowTypes : IFollowTypeProvider
    {
        
        public static readonly FollowType Channel =
            new FollowType(
                "Channel",
                "Follow Channel",
                "Folow this channel to get notified when new topics are posted.",
                "Unsubscribe",
                "You are already following this channel. Unsubscribe below...");
        
        public static readonly FollowType AllChannels =
            new FollowType(
                "AllChannels",
                "Follow All Channels",
                "Folow all channels to get notified when new topics are added to any channel..",
                "Unsubscribe",
                "You are already following all channels. Unsubscribe below...");
        
        public IEnumerable<IFollowType> GetFollowTypes()
        {
            return new[]
            {
                Channel,
                AllChannels
            };
        }

    }
}
