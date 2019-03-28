using System;
using System.Threading.Tasks;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Categories.Services
{
 
    public class EntityCategoryManager : IEntityCategoryManager
    {

        private readonly IEntityCategoryStore<EntityCategory> _entityCategoryStore;
        private readonly IBroker _broker;

        public EntityCategoryManager(
            IEntityCategoryStore<EntityCategory> entityCategoryStore,
            IBroker broker)
        {
            _entityCategoryStore = entityCategoryStore;
            _broker = broker;
        }

        public async Task<ICommandResult<EntityCategory>> CreateAsync(EntityCategory model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // We are creating - ensure we don't have an id
            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.CategoryId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CategoryId));
            }

            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }
            
            // Invoke EntityCategoryCreating subscriptions
            foreach (var handler in _broker.Pub<EntityCategory>(this, "EntityCategoryCreating"))
            {
                model = await handler.Invoke(new Message<EntityCategory>(model, this));
            }
            
            // Create result
            var result = new CommandResult<EntityCategory>();
            
            // Create relationship
            var entityCategory = await _entityCategoryStore.CreateAsync(model);
            if (entityCategory != null)
            {

                // Invoke EntityCategoryCreated subscriptions
                foreach (var handler in _broker.Pub<EntityCategory>(this, "EntityCategoryCreated"))
                {
                    entityCategory = await handler.Invoke(new Message<EntityCategory>(entityCategory, this));
                }

                // Return success
                return result.Success(entityCategory);
            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create an entity & category relationship"));
            
        }

        public async Task<ICommandResult<EntityCategory>> UpdateAsync(EntityCategory model)
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

            if (model.CategoryId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CategoryId));
            }

            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }
            
            // Invoke EntityCategoryUpdating subscriptions
            foreach (var handler in _broker.Pub<EntityCategory>(this, "EntityCategoryUpdating"))
            {
                model = await handler.Invoke(new Message<EntityCategory>(model, this));
            }

            var result = new CommandResult<EntityCategory>();

            var entityCategory = await _entityCategoryStore.UpdateAsync(model);
            if (entityCategory != null)
            {

                // Invoke EntityCategoryUpdated subscriptions
                foreach (var handler in _broker.Pub<EntityCategory>(this, "EntityCategoryUpdated"))
                {
                    entityCategory = await handler.Invoke(new Message<EntityCategory>(entityCategory, this));
                }

                // Return success
                return result.Success(entityCategory);
            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to update an entity & category relationship"));

        }

        public async Task<ICommandResult<EntityCategory>> DeleteAsync(EntityCategory model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            
            // Invoke EntityCategoryDeleting subscriptions
            foreach (var handler in _broker.Pub<EntityCategory>(this, "EntityCategoryDeleting"))
            {
                model = await handler.Invoke(new Message<EntityCategory>(model, this));
            }

            var result = new CommandResult<EntityCategory>();
            if (await _entityCategoryStore.DeleteAsync(model))
            {

                // Invoke EntityCategoryDeleted subscriptions
                foreach (var handler in _broker.Pub<EntityCategory>(this, "EntityCategoryDeleted"))
                {
                    model = await handler.Invoke(new Message<EntityCategory>(model, this));
                }

                // Return success
                return result.Success();

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete an entity & category relationship"));
            
        }

    }

}
