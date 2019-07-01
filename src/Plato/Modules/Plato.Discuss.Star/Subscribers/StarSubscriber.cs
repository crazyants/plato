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
            var entity = await _entityStore.GetByIdAsync(star.ThingId);
            if (entity == null)
            {
                return star;
            }

            // Update total stars
            entity.TotalStars = entity.TotalStars + 1;

            // Persist changes
            var updatedEntity = await _entityStore.UpdateAsync(entity);
            if (updatedEntity != null)
            {
                // Award reputation to user starring the entity
                await _reputationAwarder.AwardAsync(Reputations.StarTopic, star.CreatedUserId, "Starred a topic");

                // Award reputation to entity author when there entity is starred
                await _reputationAwarder.AwardAsync(Reputations.StarredTopic, entity.CreatedUserId, "Someone starred my topic");

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
            var entity = await _entityStore.GetByIdAsync(star.ThingId);
            if (entity == null)
            {
                return star;
            }

            // Update total stars
            entity.TotalStars = entity.TotalStars - 1;
        
            // Ensure we don't go negative
            if (entity.TotalStars < 0)
            {
                entity.TotalStars = 0;
            }
            
            // Persist changes
            var updatedEntity = await _entityStore.UpdateAsync(entity);
            if (updatedEntity != null)
            {
                // Revoke reputation from user removing the entity star
                await _reputationAwarder.RevokeAsync(Reputations.StarTopic, star.CreatedUserId, "Unstarred a topic");

                // Revoke reputation from entity author for user removing there entity star
                await _reputationAwarder.RevokeAsync(Reputations.StarredTopic, entity.CreatedUserId, "A user unstarred my topic");

            }
            
            return star;

        }

    }

}
