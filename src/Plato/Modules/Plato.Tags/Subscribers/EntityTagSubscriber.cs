using System;
using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Services;
using Plato.Tags.Stores;

namespace Plato.Tags.Subscribers
{
    public class EntityTagSubscriber<TTag> : IBrokerSubscriber where TTag : class, ITag
    {

        private readonly ITagOccurrencesUpdater<TTag> _tagOccurrencesUpdater;
        private readonly ITagStore<TTag> _tagStore;
        private readonly IBroker _broker;
        
        // Updates tag metadata whenever a entity & tag relationship is added or removed.
        public EntityTagSubscriber(
            ITagOccurrencesUpdater<TTag> tagOccurrencesUpdater,
            ITagStore<TTag> tagStore,
            IBroker broker)
        {
            _tagOccurrencesUpdater = tagOccurrencesUpdater;
            _tagStore = tagStore;
            _broker = broker;
        }

        #region "Implementation"

        public void Subscribe()
        {

            // Created
            _broker.Sub<EntityTag>(new MessageOptions()
            {
                Key = "EntityTagCreated"
            }, async message => await EntityTagCreated(message.What));

            // Deleted
            _broker.Sub<EntityTag>(new MessageOptions()
            {
                Key = "EntityTagDeleted"
            }, async message => await EntityTagDeleted(message.What));

        }

        public void Unsubscribe()
        {

            // Created
            _broker.Unsub<EntityTag>(new MessageOptions()
            {
                Key = "EntityTagCreated"
            }, async message => await EntityTagCreated(message.What));

            // Deleted
            _broker.Unsub<EntityTag>(new MessageOptions()
            {
                Key = "EntityTagDeleted"
            }, async message => await EntityTagDeleted(message.What));

        }

        #endregion

        #region "Private Methods"

        async Task<EntityTag> EntityTagCreated(EntityTag entityTag)
        {

            if (entityTag == null)
            {
                throw new ArgumentNullException(nameof(entityTag));
            }

            if (entityTag.EntityId <= 0)
            {
                return entityTag;
            }

            if (entityTag.TagId <= 0)
            {
                return entityTag;
            }

            // Get tag
            var tag = await _tagStore.GetByIdAsync(entityTag.TagId);

            // No tag found no further work needed
            if (tag == null)
            {
                return entityTag;
            }

            // Update last seen date for tag
            tag.LastSeenDate = DateTimeOffset.Now;

            // Update entity count & last seen date for tag
            await _tagOccurrencesUpdater.UpdateAsync(tag);

            return entityTag;

        }

        async Task<EntityTag> EntityTagDeleted(EntityTag entityTag)
        {

            if (entityTag == null)
            {
                throw new ArgumentNullException(nameof(entityTag));
            }

            if (entityTag.EntityId <= 0)
            {
                return entityTag;
            }

            if (entityTag.TagId <= 0)
            {
                return entityTag;
            }

            // Get tag
            var tag = await _tagStore.GetByIdAsync(entityTag.TagId);

            // No tag found no further work needed
            if (tag == null)
            {
                return entityTag;
            }
            
            // Update entity count & last seen date for tag
            await _tagOccurrencesUpdater.UpdateAsync(tag);

            return entityTag;

        }

        #endregion

    }

}
