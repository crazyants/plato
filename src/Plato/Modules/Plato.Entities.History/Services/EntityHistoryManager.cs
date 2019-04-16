using System;
using System.Threading.Tasks;
using Plato.Entities.History.Models;
using Plato.Entities.History.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Entities.History.Services
{

    public class EntityHistoryManager : IEntityHistoryManager<EntityHistory> 
    {
        
        private readonly IEntityHistoryStore<EntityHistory> _entityHistoryStore;
        private readonly IContextFacade _contextFacade;
        private readonly IBroker _broker;

        public EntityHistoryManager(
            IEntityHistoryStore<EntityHistory> entityHistoryStore,
            IContextFacade contextFacade,
            IBroker broker)
        {
            _entityHistoryStore = entityHistoryStore;
            _contextFacade = contextFacade;
            _broker = broker;
        }

        #region "Implementation"

        public async Task<ICommandResult<EntityHistory>> CreateAsync(EntityHistory model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // We should never have an Id for inserts
            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            // We always need an entityId
            if (model.EntityId <= 0)
            {
                throw new ArgumentNullException(nameof(model.EntityId));
            }

            // Ensure reply Id is 0 or above
            if (model.EntityReplyId < 0)
            {
                throw new ArgumentNullException(nameof(model.EntityReplyId));
            }

            //// We always need a CreatedUserId
            //if (model.CreatedUserId <= 0)
            //{
            //    throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            //}

            // We always need a CreatedDate
            if (model.CreatedDate == null)
            {
                throw new ArgumentNullException(nameof(model.CreatedDate));
            }
            
            // Invoke EntityHistoryCreating subscriptions
            foreach (var handler in _broker.Pub<EntityHistory>(this, "EntityHistoryCreating"))
            {
                model = await handler.Invoke(new Message<EntityHistory>(model, this));
            }

            var result = new CommandResult<EntityHistory>();

            var newHistory = await _entityHistoryStore.CreateAsync(model);
            if (newHistory != null)
            {

                // Invoke EntityHistoryCreated subscriptions
                foreach (var handler in _broker.Pub<EntityHistory>(this, "EntityHistoryCreated"))
                {
                    newHistory = await handler.Invoke(new Message<EntityHistory>(newHistory, this));
                }

                // Return success
                return result.Success(newHistory);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create the entity history entry."));
            
        }

        public async Task<ICommandResult<EntityHistory>> UpdateAsync(EntityHistory model)
        {
            
            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // We always need an Id above 0 for updates
            if (model.Id <= 0)
            {
                throw new ArgumentNullException(nameof(model.Id));
            }
            
            // We always need an entityId
            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }

            // Ensure reply Id is 0 or above
            if (model.EntityReplyId < 0)
            {
                throw new ArgumentNullException(nameof(model.EntityReplyId));
            }

            // We always need a CreatedUserId
            //if (model.CreatedUserId <= 0)
            //{
            //    throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            //}

            // We always need a CreatedDate
            if (model.CreatedDate == null)
            {
                throw new ArgumentNullException(nameof(model.CreatedDate));
            }

            // Invoke EntityHistoryUpdating subscriptions
            foreach (var handler in _broker.Pub<EntityHistory>(this, "EntityHistoryUpdating"))
            {
                model = await handler.Invoke(new Message<EntityHistory>(model, this));
            }

            var result = new CommandResult<EntityHistory>();

            var updatedHistory = await _entityHistoryStore.UpdateAsync(model);
            if (updatedHistory != null)
            {

                // Invoke EntityHistoryUpdated subscriptions
                foreach (var handler in _broker.Pub<EntityHistory>(this, "EntityHistoryUpdated"))
                {
                    updatedHistory = await handler.Invoke(new Message<EntityHistory>(updatedHistory, this));
                }

                // Return success
                return result.Success(updatedHistory);
            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to update the entity history entry"));
            
        }

        public async Task<ICommandResult<EntityHistory>> DeleteAsync(EntityHistory model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            
            // Invoke EntityHistoryDeleting subscriptions
            foreach (var handler in _broker.Pub<EntityHistory>(this, "EntityHistoryDeleting"))
            {
                model = await handler.Invoke(new Message<EntityHistory>(model, this));
            }
            
            var result = new CommandResult<EntityHistory>();
            if (await _entityHistoryStore.DeleteAsync(model))
            {

                // Invoke EntityHistoryDeleted subscriptions
                foreach (var handler in _broker.Pub<EntityHistory>(this, "EntityHistoryDeleted"))
                {
                    model = await handler.Invoke(new Message<EntityHistory>(model, this));
                }

                // Return success
                return result.Success();

            }
            
            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete the entity history entry"));
            
        }

        #endregion
        
    }

}
