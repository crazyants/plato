using System;
using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Reputations.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Stores;

namespace Plato.Follow.Tags.Subscribers
{
    public class FollowSubscriber : IBrokerSubscriber
    {

        private readonly ITagStore<Tag> _tagStore;
        private readonly IBroker _broker;
        private readonly IUserReputationAwarder _reputationAwarder;

        public FollowSubscriber(
            IBroker broker,
            IUserReputationAwarder reputationAwarder,
            ITagStore<Tag> tagStore)
        {
            _broker = broker;
            _reputationAwarder = reputationAwarder;
            _tagStore = tagStore;
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
            
            // Ensure the tag we are following still exists
            var tag = await _tagStore.GetByIdAsync(follow.ThingId);
            if (tag == null)
            {
                return follow;
            }

            // Update total follows
            tag.TotalFollows = tag.TotalFollows + 1;

            // Persist changes
            var updatedTag = await _tagStore.UpdateAsync(tag);
            if (updatedTag != null)
            {

                // Award reputation for following tag
                await _reputationAwarder.AwardAsync(Reputations.NewFollow, follow.CreatedUserId);

            }
            
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

            // Ensure the tag we are following still exists
            var tag = await _tagStore.GetByIdAsync(follow.ThingId);
            if (tag == null)
            {
                return follow;
            }

            // Update total follows
            tag.TotalFollows = tag.TotalFollows - 1;
        
            // Ensure we don't go negative
            if (tag.TotalFollows < 0)
            {
                tag.TotalFollows = 0;
            }
            
            // Persist changes
            var updatedTag = await _tagStore.UpdateAsync(tag);
            if (updatedTag != null)
            {
                // Revoke reputation for following tag
                await _reputationAwarder.RevokeAsync(Reputations.NewFollow, follow.CreatedUserId);
            }
            
            return follow;

        }

    }

}
