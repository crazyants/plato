using System;
using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Reputations.Abstractions;
using Plato.Tags.Stores;
using Plato.Internal.Models.Tags;

namespace Plato.Discuss.Tags.Follow.Subscribers
{
    public class FollowSubscriber : IBrokerSubscriber
    {

        private readonly ITagStore<TagBase> _tagStore;
        private readonly IBroker _broker;
        private readonly IUserReputationAwarder _reputationAwarder;

        public FollowSubscriber(
            IBroker broker,
            IUserReputationAwarder reputationAwarder,
            ITagStore<TagBase> tagStore)
        {
            _broker = broker;
            _reputationAwarder = reputationAwarder;
            _tagStore = tagStore;
        }

        public void Subscribe()
        {
 
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
            // Add a reputation for new follows
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
                await _reputationAwarder.AwardAsync(Reputations.NewFollow, follow.CreatedUserId, $"Followed tag \"{tag.Name}\"");

            }
            
            return follow;

        }

        private async Task<Follows.Models.Follow> FollowDeleted(Follows.Models.Follow follow)
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
                await _reputationAwarder.RevokeAsync(Reputations.NewFollow, follow.CreatedUserId, $"Unfollowed tag \"{tag.Name}\"");
            }
            
            return follow;

        }

    }

}
