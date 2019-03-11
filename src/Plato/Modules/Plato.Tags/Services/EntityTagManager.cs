using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Stores;

namespace Plato.Tags.Services
{

    public class EntityTagManager : IEntityTagManager<EntityTag>
    {

        private readonly IEntityTagStore<EntityTag> _entityTagStore;
    
        private readonly IBroker _broker;

        public EntityTagManager(
            IEntityTagStore<EntityTag> entityTagStore,
            IBroker broker)
        {
            _entityTagStore = entityTagStore;
            _broker = broker;
        }

        public async Task<ICommandResult<EntityTag>> CreateAsync(EntityTag model)
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
            
            if (model.TagId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.TagId));
            }

            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }


            if (model.CreatedDate == null)
            {
                throw new ArgumentNullException(nameof(model.CreatedDate));
            }

            // Invoke EntityTagCreating subscriptions
            foreach (var handler in _broker.Pub<EntityTag>(this, "EntityTagCreating"))
            {
                model = await handler.Invoke(new Message<EntityTag>(model, this));
            }

            // Create result
            var result = new CommandResult<EntityTag>();

            // Persist to database
            var newEntityTag = await _entityTagStore.CreateAsync(model);
            if (newEntityTag != null)
            {

                // Invoke EntityTagCreated subscriptions
                foreach (var handler in _broker.Pub<EntityTag>(this, "EntityTagCreated"))
                {
                    newEntityTag = await handler.Invoke(new Message<EntityTag>(newEntityTag, this));
                }
                
                // Return success
                return result.Success(newEntityTag);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create an entity / label relatinship"));
            
        }

        public async Task<ICommandResult<EntityTag>> UpdateAsync(EntityTag model)
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

            if (model.TagId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.TagId));
            }
            
            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }


            if (model.CreatedDate == null)
            {
                throw new ArgumentNullException(nameof(model.CreatedDate));
            }
            
            // Invoke EntityTagUpdating subscriptions
            foreach (var handler in _broker.Pub<EntityTag>(this, "EntityTagUpdating"))
            {
                model = await handler.Invoke(new Message<EntityTag>(model, this));
            }
            
            // Create result
            var result = new CommandResult<EntityTag>();

            // Persist to database
            var updatedEntityTag = await _entityTagStore.UpdateAsync(model);
            if (updatedEntityTag != null)
            {

                // Invoke EntityTagUpdated subscriptions
                foreach (var handler in _broker.Pub<EntityTag>(this, "EntityTagUpdated"))
                {
                    updatedEntityTag = await handler.Invoke(new Message<EntityTag>(updatedEntityTag, this));
                }

                // Return success
                return result.Success(updatedEntityTag);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create an entity / label relatinship"));
            
        }

        public async Task<ICommandResult<EntityTag>> DeleteAsync(EntityTag model)
        {
            
            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Invoke EntityTagDeleting subscriptions
            foreach (var handler in _broker.Pub<EntityTag>(this, "EntityTagDeleting"))
            {
                model = await handler.Invoke(new Message<EntityTag>(model, this));
            }

            var result = new CommandResult<EntityTag>();
            if (await _entityTagStore.DeleteAsync(model))
            {

                // Invoke EntityTagDeleted subscriptions
                foreach (var handler in _broker.Pub<EntityTag>(this, "EntityTagDeleted"))
                {
                    model = await handler.Invoke(new Message<EntityTag>(model, this));
                }

                // Return success
                return result.Success();

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete the entity / label relationship"));


        }

    }

}
