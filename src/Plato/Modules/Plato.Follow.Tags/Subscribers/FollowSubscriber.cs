using System;
using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Follow.Tags.Subscribers
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
 
            _broker.Sub<Models.Follow>(new MessageOptions()
            {
                Key = "FollowCreated"
            }, async message => await FollowCreated(message.What));
            
            _broker.Sub<Models.Follow>(new MessageOptions()
            {
                Key = "FollowDeleted"
            }, async message => await FollowDeleted(message.What));

        }

        public void Unsubscribe()
        {
            // Add a reputation for new follows
            _broker.Unsub<Models.Follow>(new MessageOptions()
            {
                Key = "FollowCreated"
            }, async message => await FollowCreated(message.What));

            _broker.Unsub<Models.Follow>(new MessageOptions()
            {
                Key = "FollowDeleted"
            }, async message => await FollowDeleted(message.What));

        }

        private async Task<Models.Follow> FollowCreated(Models.Follow follow)
        {

            if (follow == null)
            {
                return null;
            }

            // Is this a tag follow?
            if (!follow.Name.Equals(FollowTypes.Tag.Name, StringComparison.OrdinalIgnoreCase))
            {
                return follow;
            }
            
            // Award reputation for following tag
            await _reputationAwarder.AwardAsync(Reputations.NewFollow, follow.CreatedUserId);
            
            return follow;

        }

        private async Task<Models.Follow> FollowDeleted(Models.Follow follow)
        {

            if (follow == null)
            {
                return null;
            }

            // Is this a tag follow?
            if (!follow.Name.Equals(FollowTypes.Tag.Name, StringComparison.OrdinalIgnoreCase))
            {
                return follow;
            }

            // Revoke reputation for following tag
            await _reputationAwarder.RevokeAsync(Reputations.NewFollow, follow.CreatedUserId);

            return follow;

        }

    }
}
