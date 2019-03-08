using System;
using System.Threading.Tasks;
using Plato.Entities.Labels.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Entities.Labels.Models;

namespace Plato.Entities.Labels.Services
{
    
    public class EntityLabelManager : IEntityLabelManager<EntityLabel>
    {

        private readonly IEntityLabelStore<EntityLabel> _entityLabelStore;

        private readonly IBroker _broker;

        public EntityLabelManager(
            IEntityLabelStore<EntityLabel> entityLabelStore,
            IBroker broker)
        {
            _entityLabelStore = entityLabelStore;
            _broker = broker;
        }

        public async Task<ICommandResult<EntityLabel>> CreateAsync(EntityLabel model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
             
            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }
            
            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }
            
            if (model.LabelId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.LabelId));
            }

            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }

            if (model.CreatedDate == null)
            {
                throw new ArgumentNullException(nameof(model.CreatedDate));
            }
            
            // Invoke EntityLabelCreating subscriptions
            foreach (var handler in _broker.Pub<EntityLabel>(this, "EntityLabelCreating"))
            {
                model = await handler.Invoke(new Message<EntityLabel>(model, this));
            }

            // Create result
            var result = new CommandResult<EntityLabel>();

            // Persist to database
            var newEntityLabel = await _entityLabelStore.CreateAsync(model);
            if (newEntityLabel != null)
            {

                // Invoke EntityLabelCreated subscriptions
                foreach (var handler in _broker.Pub<EntityLabel>(this, "EntityLabelCreated"))
                {
                    newEntityLabel = await handler.Invoke(new Message<EntityLabel>(newEntityLabel, this));
                }
                
                // Return success
                return result.Success(newEntityLabel);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create an entity / label relatinship"));
            
        }

        public async Task<ICommandResult<EntityLabel>> UpdateAsync(EntityLabel model)
        {
            
            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }

            if (model.LabelId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.LabelId));
            }

            if (model.ModifiedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.ModifiedUserId));
            }

            if (model.ModifiedDate == null)
            {
                throw new ArgumentNullException(nameof(model.ModifiedDate));
            }


            // Invoke EntityLabelUpdating subscriptions
            foreach (var handler in _broker.Pub<EntityLabel>(this, "EntityLabelUpdating"))
            {
                model = await handler.Invoke(new Message<EntityLabel>(model, this));
            }
            
            // Create result
            var result = new CommandResult<EntityLabel>();

            // Persist to database
            var updatedEntityLabel = await _entityLabelStore.UpdateAsync(model);
            if (updatedEntityLabel != null)
            {

                // Invoke EntityLabelUpdated subscriptions
                foreach (var handler in _broker.Pub<EntityLabel>(this, "EntityLabelUpdated"))
                {
                    updatedEntityLabel = await handler.Invoke(new Message<EntityLabel>(updatedEntityLabel, this));
                }

                // Return success
                return result.Success(updatedEntityLabel);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create an entity / label relatinship"));
            
        }

        public async Task<ICommandResult<EntityLabel>> DeleteAsync(EntityLabel model)
        {
            
            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Invoke EntityLabelDeleting subscriptions
            foreach (var handler in _broker.Pub<EntityLabel>(this, "EntityLabelDeleting"))
            {
                model = await handler.Invoke(new Message<EntityLabel>(model, this));
            }

            var result = new CommandResult<EntityLabel>();
            if (await _entityLabelStore.DeleteAsync(model))
            {
             
                // Invoke EntityLabelDeleted subscriptions
                foreach (var handler in _broker.Pub<EntityLabel>(this, "EntityLabelDeleted"))
                {
                    model = await handler.Invoke(new Message<EntityLabel>(model, this));
                }

                // Return success
                return result.Success();

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete the entity / label relationship"));


        }

    }

}
