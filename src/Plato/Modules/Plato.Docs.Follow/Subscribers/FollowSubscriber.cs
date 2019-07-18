using System;
using System.Threading.Tasks;
using Plato.Docs.Models;
using Plato.Entities.Stores;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Docs.Follow.Subscribers
{
    public class FollowSubscriber : IBrokerSubscriber
    {

        private readonly IUserReputationAwarder _reputationAwarder;
        private readonly IEntityStore<Doc> _entityStore;
        private readonly IBroker _broker;

        public FollowSubscriber(
            IUserReputationAwarder reputationAwarder,
            IEntityStore<Doc> entityStore,
            IBroker broker)
        {
            _reputationAwarder = reputationAwarder;
            _entityStore = entityStore;
            _broker = broker;
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
            if (!follow.Name.Equals(FollowTypes.Doc.Name, StringComparison.OrdinalIgnoreCase))
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
                await _reputationAwarder.AwardAsync(Reputations.NewFollow, follow.CreatedUserId, "Followed a doc");
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
            if (!follow.Name.Equals(FollowTypes.Doc.Name, StringComparison.OrdinalIgnoreCase))
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
                await _reputationAwarder.RevokeAsync(Reputations.NewFollow, follow.CreatedUserId, "Unfollowed a doc");
            }
            
            return follow;

        }

    }

}
