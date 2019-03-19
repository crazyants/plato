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

        private readonly ILabelStore<LabelBase> _labelStore;
        private readonly ILabelManager<LabelBase> _labelManager;
        private readonly IBroker _broker;

        /// <summary>
        /// Updates label metadata whenever a entity & label relationship is added or removed.
        /// </summary>
        /// <param name="labelManager"></param>
        /// <param name="labelStore"></param>
        /// <param name="broker"></param>
        public EntityLabelSubscriber(
            ILabelManager<LabelBase> labelManager,
            ILabelStore<LabelBase> labelStore,
            IBroker broker)
        {
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
            
            // Update label
            label.TotalEntities = label.TotalEntities + 1;
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

            // Update label
            label.TotalEntities = label.TotalEntities - 1;

            // Ensure we never go negative
            if (label.TotalEntities < 0)
            {
                label.TotalEntities = 0;
            }

            // Persist label updates
            await _labelManager.UpdateAsync(label);

            return entityLabel;

        }
        
        #endregion

    }
}
