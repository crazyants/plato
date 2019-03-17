using System;
using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Tags;
using Plato.Tags.Models;
using Plato.Tags.Services;
using Plato.Tags.Stores;

namespace Plato.Tags.Subscribers
{
    public class EntityTagSubscriber : IBrokerSubscriber
    {

        private readonly ITagStore<TagBase> _tagStore;
        private readonly ITagManager<TagBase> _tagManager;
        private readonly IBroker _broker;
        private readonly IEntityTagStore<EntityTag> _entityTagStore;

        /// <summary>
        /// Updates tag metadata whenever a entity & tag relationship is added or removed.
        /// </summary>
        /// <param name="tagManager"></param>
        /// <param name="tagStore"></param>
        /// <param name="broker"></param>
        public EntityTagSubscriber(
            ITagManager<TagBase> tagManager,
            ITagStore<TagBase> tagStore,
            IBroker broker, 
            IEntityTagStore<EntityTag> entityTagStore)
        {
            _tagManager = tagManager;
            _tagStore = tagStore;
            _broker = broker;
            _entityTagStore = entityTagStore;
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

            // Get count for entities & replies tagged with this tag
            var entityTags = await _entityTagStore.QueryAsync()
                .Take(1)
                .Select<EntityTagQueryParams>(q =>
                {
                    q.TagId.Equals(tag.Id);
                })
                .ToList();
            
            // Update 
            tag.TotalEntities = entityTags?.Total ?? 0;
            tag.LastSeenDate = DateTimeOffset.Now;

            // Persist 
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
