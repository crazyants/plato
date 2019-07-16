using System;
using System.Threading.Tasks;
using Plato.Ideas.Models;
using Plato.Entities.Stores;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Ideas.Follow.Subscribers
{
    public class FollowSubscriber : IBrokerSubscriber
    {

        private readonly IEntityStore<Idea> _entityStore;
        private readonly IBroker _broker;
        private readonly IUserReputationAwarder _reputationAwarder;

        public FollowSubscriber(
            IBroker broker,
            IUserReputationAwarder reputationAwarder,
            IEntityStore<Idea> entityStore)
        {
            _broker = broker;
            _reputationAwarder = reputationAwarder;
            _entityStore = entityStore;
        }

        public void Subscribe()
        {
 
            _broker.Sub<Plato.Follows.Models.Follow>(new MessageOptions()
            {
                Key = "FollowCreated"
            }, async message => await FollowCreated(message.What));
            
            _broker.Sub<Plato.Follows.Models.Follow>(new MessageOptions()
            {
                Key = "FollowDeleted"
            }, async message => await FollowDeleted(message.What));

        }

        public void Unsubscribe()
        {
            // Add a reputation for new follows
            _broker.Unsub<Plato.Follows.Models.Follow>(new MessageOptions()
            {
                Key = "FollowCreated"
            }, async message => await FollowCreated(message.What));

            _broker.Unsub<Plato.Follows.Models.Follow>(new MessageOptions()
            {
                Key = "FollowDeleted"
            }, async message => await FollowDeleted(message.What));

        }

        private async Task<Plato.Follows.Models.Follow> FollowCreated(Plato.Follows.Models.Follow follow)
        {

            if (follow == null)
            {
                return null;
            }

            // Is this a tag follow?
            if (!follow.Name.Equals(FollowTypes.Idea.Name, StringComparison.OrdinalIgnoreCase))
            {
                return follow;
            }
            
            // Ensure the topic we are following still exists
            var existingTopic = await _entityStore.GetByIdAsync(follow.ThingId);
            if (existingTopic == null)
            {
                return follow;
            }

            // Update total follows
            existingTopic.TotalFollows = existingTopic.TotalFollows + 1;

            // Persist changes
            var updatedTopic = await _entityStore.UpdateAsync(existingTopic);
            if (updatedTopic != null)
            {
                // Award reputation for following topic
                await _reputationAwarder.AwardAsync(Reputations.NewFollow, follow.CreatedUserId, "Followed a topic");
            }
            
            return follow;

        }

        private async Task<Plato.Follows.Models.Follow> FollowDeleted(Plato.Follows.Models.Follow follow)
        {

            if (follow == null)
            {
                return null;
            }

            // Is this a topic follow?
            if (!follow.Name.Equals(FollowTypes.Idea.Name, StringComparison.OrdinalIgnoreCase))
            {
                return follow;
            }

            // Ensure the topic we are following still exists
            var existingTopic = await _entityStore.GetByIdAsync(follow.ThingId);
            if (existingTopic == null)
            {
                return follow;
            }

            // Update total follows
            existingTopic.TotalFollows = existingTopic.TotalFollows - 1;
        
            // Ensure we don't go negative
            if (existingTopic.TotalFollows < 0)
            {
                existingTopic.TotalFollows = 0;
            }
            
            // Persist changes
            var updatedTopic = await _entityStore.UpdateAsync(existingTopic);
            if (updatedTopic != null)
            {
                // Revoke reputation for following tag
                await _reputationAwarder.RevokeAsync(Reputations.NewFollow, follow.CreatedUserId, "Unfollowed a topic");
            }
            
            return follow;

        }

    }

}
