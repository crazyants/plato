using System;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Discuss.Star.Subscribers
{
    public class StarSubscriber : IBrokerSubscriber
    {

        private readonly IEntityStore<Topic> _entityStore;
        private readonly IBroker _broker;
        private readonly IUserReputationAwarder _reputationAwarder;

        public StarSubscriber(
            IBroker broker,
            IUserReputationAwarder reputationAwarder,
            IEntityStore<Topic> entityStore)
        {
            _broker = broker;
            _reputationAwarder = reputationAwarder;
            _entityStore = entityStore;
        }

        public void Subscribe()
        {
 
            // Created
            _broker.Sub<Stars.Models.Star>(new MessageOptions()
            {
                Key = "StarCreated"
            }, async message => await StarCreated(message.What));
            
            // Updated
            _broker.Sub<Stars.Models.Star>(new MessageOptions()
            {
                Key = "StarDeleted"
            }, async message => await StarDeleted(message.What));

        }

        public void Unsubscribe()
        {
            // Created
            _broker.Unsub<Stars.Models.Star>(new MessageOptions()
            {
                Key = "StarCreated"
            }, async message => await StarCreated(message.What));

            // Updated
            _broker.Unsub<Stars.Models.Star>(new MessageOptions()
            {
                Key = "StarDeleted"
            }, async message => await StarDeleted(message.What));

        }

        private async Task<Stars.Models.Star> StarCreated(Stars.Models.Star star)
        {

            if (star == null)
            {
                return null;
            }

            // Is this a topic star?
            if (!star.Name.Equals(StarTypes.Topic.Name, StringComparison.OrdinalIgnoreCase))
            {
                return star;
            }
            
            // Ensure the topic we are starring exists
            var topic = await _entityStore.GetByIdAsync(star.ThingId);
            if (topic == null)
            {
                return star;
            }

            // Update total stars
            topic.TotalStars = topic.TotalStars + 1;

            // Persist changes
            var updatedTopic = await _entityStore.UpdateAsync(topic);
            if (updatedTopic != null)
            {
                // Award reputation to user starring the topic
                await _reputationAwarder.AwardAsync(Reputations.StarTopic, star.CreatedUserId);

                // Award reputation to topic author when there topic is starred
                await _reputationAwarder.AwardAsync(Reputations.StarredTopic, topic.CreatedUserId);

            }

            return star;

        }

        private async Task<Stars.Models.Star> StarDeleted(Stars.Models.Star star)
        {

            if (star == null)
            {
                return null;
            }

            // Is this a topic star?
            if (!star.Name.Equals(StarTypes.Topic.Name, StringComparison.OrdinalIgnoreCase))
            {
                return star;
            }

            // Ensure the topic we are starring exists
            var topic = await _entityStore.GetByIdAsync(star.ThingId);
            if (topic == null)
            {
                return star;
            }

            // Update total stars
            topic.TotalStars = topic.TotalStars - 1;
        
            // Ensure we don't go negative
            if (topic.TotalStars < 0)
            {
                topic.TotalStars = 0;
            }
            
            // Persist changes
            var updatedTopic = await _entityStore.UpdateAsync(topic);
            if (updatedTopic != null)
            {
                // Revoke reputation from user removing the topic star
                await _reputationAwarder.RevokeAsync(Reputations.StarTopic, star.CreatedUserId);

                // Revoke reputation from topic author for user removing there topic star
                await _reputationAwarder.RevokeAsync(Reputations.StarredTopic, topic.CreatedUserId);

            }
            
            return star;

        }

    }

}
