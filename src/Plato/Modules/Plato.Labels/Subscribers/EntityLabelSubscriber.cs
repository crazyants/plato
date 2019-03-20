using System;
using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;
using Plato.Labels.Models;
using Plato.Labels.Services;
using Plato.Labels.Stores;

namespace Plato.Labels.Subscribers
{
    public class EntityLabelSubscriber : IBrokerSubscriber
    {

        private readonly IEntityLabelStore<EntityLabel> _entityLabelStore;
        private readonly ILabelManager<LabelBase> _labelManager;
        private readonly ILabelStore<LabelBase> _labelStore;
        private readonly IBroker _broker;
        
        // Updates label metadata whenever a entity & label relationship is added or removed.
        public EntityLabelSubscriber(
            IEntityLabelStore<EntityLabel> entityLabelStore,
            ILabelManager<LabelBase> labelManager,
            ILabelStore<LabelBase> labelStore,
            IBroker broker)
        {
            _entityLabelStore = entityLabelStore;
            _labelManager = labelManager;
            _labelStore = labelStore;
            _broker = broker;
        }

        #region "Implementation"

        public void Subscribe()
        {

            // Created
            _broker.Sub<EntityLabel>(new MessageOptions()
            {
                Key = "EntityLabelCreated"
            }, async message => await EntityLabelCreated(message.What));
            
            // Deleted
            _broker.Sub<EntityLabel>(new MessageOptions()
            {
                Key = "EntityLabelDeleted"
            }, async message => await EntityLabelDeleted(message.What));

        }

        public void Unsubscribe()
        {

            // Created
            _broker.Unsub<EntityLabel>(new MessageOptions()
            {
                Key = "EntityLabelCreated"
            }, async message => await EntityLabelCreated(message.What));

            // Deleted
            _broker.Unsub<EntityLabel>(new MessageOptions()
            {
                Key = "EntityLabelDeleted"
            }, async message => await EntityLabelDeleted(message.What));

        }

        #endregion

        #region "Private Methods"

        async Task<EntityLabel> EntityLabelCreated(EntityLabel entityLabel)
        {

            if (entityLabel == null)
            {
                throw new ArgumentNullException(nameof(entityLabel));
            }
            
            if (entityLabel.EntityId <= 0)
            {
                return entityLabel;
            }

            if (entityLabel.LabelId <= 0)
            {
                return entityLabel;
            }
            
            // Get label
            var label = await _labelStore.GetByIdAsync(entityLabel.LabelId);

            // No label found no further work needed
            if (label == null)
            {
                return entityLabel;
            }

            // Get count for entities labeled with this label
            var entityLabels = await _entityLabelStore.QueryAsync()
                .Take(1)
                .Select<EntityLabelQueryParams>(q =>
                {
                    q.LabelId.Equals(label.Id);
                })
                .ToList();
            
            // Update label
            label.TotalEntities = entityLabels?.Total ?? 0;
            label.LastEntityDate = DateTimeOffset.UtcNow;

            // Persist label updates
            await _labelManager.UpdateAsync(label);

            return entityLabel;

        }

        async Task<EntityLabel> EntityLabelDeleted(EntityLabel entityLabel)
        {

            if (entityLabel == null)
            {
                throw new ArgumentNullException(nameof(entityLabel));
            }

            if (entityLabel.EntityId <= 0)
            {
                return entityLabel;
            }
            
            if (entityLabel.LabelId <= 0)
            {
                return entityLabel;
            }
            
            // Get label
            var label = await _labelStore.GetByIdAsync(entityLabel.LabelId);

            // No label found no further work needed
            if (label == null)
            {
                return entityLabel;
            }
            
            // Get count for entities labeled with this label
            var entityLabels = await _entityLabelStore.QueryAsync()
                .Take(1)
                .Select<EntityLabelQueryParams>(q =>
                {
                    q.LabelId.Equals(label.Id);
                })
                .ToList();

            // Update label
            label.TotalEntities = entityLabels?.Total ?? 0;
            
            // Persist label updates
            await _labelManager.UpdateAsync(label);

            return entityLabel;

        }
        
        #endregion

    }
}
