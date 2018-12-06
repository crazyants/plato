using System;
using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Services;
using Plato.Tags.Stores;

namespace Plato.Tags.Subscribers
{
    public class EntityTagSubscriber : IBrokerSubscriber
    {

        private readonly ITagStore<Tag> _tagStore;
        private readonly ITagManager<Tag> _tagManager;
        private readonly IBroker _broker;

        /// <summary>
        /// Updates tag metadata whenever a entity & tag relationship is added or removed.
        /// </summary>
        /// <param name="tagManager"></param>
        /// <param name="tagStore"></param>
        /// <param name="broker"></param>
        public EntityTagSubscriber(
            ITagManager<Tag> tagManager,
            ITagStore<Tag> tagStore,
            IBroker broker)
        {
            _tagManager = tagManager;
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

            // Update tag
            tag.TotalEntities = tag.TotalEntities + 1;
            tag.LastSeenDate = DateTimeOffset.Now;

            // Persist label updates
            await _tagManager.UpdateAsync(tag);

            return entityTag;

        }

        async Task<EntityTag> EntityTagDeleted(EntityTag entityLabel)
        {

            if (entityLabel == null)
            {
                throw new ArgumentNullException(nameof(entityLabel));
            }

            if (entityLabel.EntityId <= 0)
            {
                return entityLabel;
            }

            if (entityLabel.TagId <= 0)
            {
                return entityLabel;
            }

            // Get label
            var label = await _tagStore.GetByIdAsync(entityLabel.TagId);

            // No tag found no further work needed
            if (label == null)
            {
                return entityLabel;
            }

            // Update tag
            label.TotalEntities = label.TotalEntities - 1;

            // Ensure we never go negative
            if (label.TotalEntities < 0)
            {
                label.TotalEntities = 0;
            }

            // Persist updates
            await _tagManager.UpdateAsync(label);

            return entityLabel;

        }

        #endregion

    }

}
