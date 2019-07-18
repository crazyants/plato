using System;
using System.Threading.Tasks;
using Plato.Discuss.Labels.Models;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Reputations.Abstractions;
using Plato.Labels.Stores;

namespace Plato.Discuss.Labels.Follow.Subscribers
{
    public class FollowSubscriber : IBrokerSubscriber
    {

        private readonly IUserReputationAwarder _reputationAwarder;
        private readonly ILabelStore<Label> _labelStore;
        private readonly IBroker _broker;

        public FollowSubscriber(
            IUserReputationAwarder reputationAwarder,
            ILabelStore<Label> labelStore,
            IBroker broker)
        {
            _reputationAwarder = reputationAwarder;
            _labelStore = labelStore;
            _broker = broker;
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

            // Is this a label follow?
            if (!follow.Name.Equals(FollowTypes.Label.Name, StringComparison.OrdinalIgnoreCase))
            {
                return follow;
            }
            
            // Get the label we are following
            var label = await _labelStore.GetByIdAsync(follow.ThingId);
            if (label == null)
            {
                return follow;
            }

            // Update follow count
            label.TotalFollows = label.TotalFollows + 1;

            // Persist changes
            var updatedLabel = await _labelStore.UpdateAsync(label);
            if (updatedLabel != null)
            {
                // Award reputation for following label
                await _reputationAwarder.AwardAsync(Reputations.NewFollow, follow.CreatedUserId, $"Followed label \"{label.Name}\"");
            }

            return follow;

        }

        private async Task<Follows.Models.Follow> FollowDeleted(Follows.Models.Follow follow)
        {

            if (follow == null)
            {
                return null;
            }

            // Is this a discuss label follow?
            if (!follow.Name.Equals(FollowTypes.Label.Name, StringComparison.OrdinalIgnoreCase))
            {
                return follow;
            }

            // Get the label we are following
            var label = await _labelStore.GetByIdAsync(follow.ThingId);
            if (label == null)
            {
                return follow;
            }

            // Update follow count
            label.TotalFollows = label.TotalFollows - 1;

            // Ensure we don't go negative
            if (label.TotalFollows < 0)
            {
                label.TotalFollows = 0;
            }

            // Persist changes
            var updatedLabel = await _labelStore.UpdateAsync(label);
            if (updatedLabel != null)
            {
                // Revoke reputation for following tag
                await _reputationAwarder.RevokeAsync(Reputations.NewFollow, follow.CreatedUserId, $"Unfollowed label \"{label.Name}\"");
            }
            
            return follow;

        }

    }
}
