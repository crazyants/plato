using System;
using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Follow.Users.Subscribers
{
    public class FollowSubscriber : IBrokerSubscriber
    {

        private readonly IBroker _broker;
        private readonly IUserReputationAwarder _reputationAwarder;

        public FollowSubscriber(
            IBroker broker,
            IUserReputationAwarder reputationAwarder)
        {
            _broker = broker;
            _reputationAwarder = reputationAwarder;
        }

        public void Subscribe()
        {
            // Add a reputation for new follows
            _broker.Sub<Follows.Models.Follow>(new MessageOptions()
            {
                Key = "FollowCreated"
            }, async message => await FollowCreated(message.What));

            _broker.Sub<Follows.Models.Follow>(new MessageOptions()
            {
                Key = "FollowDeleted"
            }, async message => await FollowDeleted(message.What));

        }

        public void Unsubscribe()
        {
      
            _broker.Unsub<Follows.Models.Follow>(new MessageOptions()
            {
                Key = "FollowCreated"
            }, async message => await FollowCreated(message.What));

            _broker.Unsub<Follows.Models.Follow>(new MessageOptions()
            {
                Key = "FollowDeleted"
            }, async message => await FollowDeleted(message.What));

        }

        private async Task<Follows.Models.Follow> FollowCreated(Follows.Models.Follow follow)
        {

            if (follow == null)
            {
                return null;
            }

            // Is this a user follow?
            if (!follow.Name.Equals(FollowTypes.User.Name, StringComparison.OrdinalIgnoreCase))
            {
                return follow;
            }



            // Award new follow reputation to the user following another user
            await _reputationAwarder.AwardAsync(Reputations.NewFollow, follow.CreatedUserId, "Followed a user");

            // Award new follower reputation to the user the current user is following
            await _reputationAwarder.AwardAsync(Reputations.NewFollower, follow.ThingId, "Received a follower");
            
            return follow;

        }

        private async Task<Follows.Models.Follow> FollowDeleted(Follows.Models.Follow follow)
        {

            if (follow == null)
            {
                return null;
            }

            // Is this a user follow?
            if (!follow.Name.Equals(FollowTypes.User.Name, StringComparison.OrdinalIgnoreCase))
            {
                return follow;
            }

            // Award new follow reputation to the user following another user
            await _reputationAwarder.RevokeAsync(Reputations.NewFollow, follow.CreatedUserId, "Removed Follow");

            // Award new follower reputation to the user the current user is following
            await _reputationAwarder.RevokeAsync(Reputations.NewFollower, follow.ThingId, "Follower Removed");

            return follow;

        }

    }
}
