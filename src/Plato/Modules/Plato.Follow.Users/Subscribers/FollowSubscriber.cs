using System;
using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Reputations.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Follow.Users.Subscribers
{
    public class FollowSubscriber : IBrokerSubscriber
    {

        private readonly IBroker _broker;
        private readonly IUserReputationAwarder _reputationAwarder;
        private readonly IPlatoUserStore<User> _platoUserStore;

        public FollowSubscriber(
            IBroker broker,
            IUserReputationAwarder reputationAwarder,
            IPlatoUserStore<User> platoUserStore)
        {
            _broker = broker;
            _reputationAwarder = reputationAwarder;
            _platoUserStore = platoUserStore;
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

            // Get the user we are following
            var user = await _platoUserStore.GetByIdAsync(follow.ThingId);
            if (user == null)
            {
                return follow;
            }

            // Award new follow reputation to the user following another user
            await _reputationAwarder.AwardAsync(Reputations.NewFollow, follow.CreatedUserId, $"Following user \"{user.DisplayName}");

            // Award new follower reputation to the user the current user is following
            await _reputationAwarder.AwardAsync(Reputations.NewFollower, follow.ThingId, $"{follow.CreatedBy.DisplayName} is following me");
            
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


            // Get the user we are following
            var user = await _platoUserStore.GetByIdAsync(follow.ThingId);
            if (user == null)
            {
                return follow;
            }
            
            // Revoke follow reputation to the user following another user
            await _reputationAwarder.RevokeAsync(Reputations.NewFollow, follow.CreatedUserId, $"Unfollowed user \"{user.DisplayName}");

            // Revoke follower reputation for the user the current user is following
            await _reputationAwarder.RevokeAsync(Reputations.NewFollower, follow.ThingId, $"{follow.CreatedBy.DisplayName} stopped following me");

            return follow;

        }

    }
}
