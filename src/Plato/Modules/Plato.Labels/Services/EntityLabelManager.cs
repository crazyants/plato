using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Labels.Models;
using Plato.Labels.Stores;

namespace Plato.Labels.Services
{
    
    public class EntityLabelManager : IEntityLabelManager<EntityLabel>
    {

        private readonly IEntityLabelStore<EntityLabel> _entityLabelStore;
        private readonly IContextFacade _contextFacade;
        private readonly IBroker _broker;

        public EntityLabelManager(
            IEntityLabelStore<EntityLabel> entityLabelStore,
            IContextFacade contextFacade,
            IBroker broker)
        {
            _entityLabelStore = entityLabelStore;
            _broker = broker;
            _contextFacade = contextFacade;
        }

        public async Task<IActivityResult<EntityLabel>> CreateAsync(EntityLabel model)
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

            // Update
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (model.CreatedUserId == 0)
            {
                model.CreatedUserId = user?.Id ?? 0;
            }

            model.CreatedDate = DateTime.UtcNow;

            // Publish EntityLabelCreating event
            _broker.Pub<EntityLabel>(this, new MessageOptions()
            {
                Key = "EntityLabelCreating"
            }, model);

            // Create result
            var result = new ActivityResult<EntityLabel>();

            // Persist to database
            var newEntityLabel = await _entityLabelStore.CreateAsync(model);
            if (newEntityLabel != null)
            {
                // Publish EntityLabelCreated event
                _broker.Pub<EntityLabel>(this, new MessageOptions()
                {
                    Key = "EntityLabelCreated"
                }, newEntityLabel);

                // Return success
                return result.Success(newEntityLabel);

            }

            return result.Failed(new ActivityError("An unknown error occurred whilst attempting to create an entity / label relatinship"));


        }

        public async Task<IActivityResult<EntityLabel>> UpdateAsync(EntityLabel model)
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

            // Update
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (model.ModifiedUserId == 0)
            {
                model.ModifiedUserId = user?.Id ?? 0;
            }

            model.ModifiedDate = DateTime.UtcNow;

            // Publish EntityLabelUpdating event
            _broker.Pub<EntityLabel>(this, new MessageOptions()
            {
                Key = "EntityLabelUpdating"
            }, model);

            // Create result
            var result = new ActivityResult<EntityLabel>();

            // Persist to database
            var updatedEntityLabel = await _entityLabelStore.CreateAsync(model);
            if (updatedEntityLabel != null)
            {
                // Publish EntityLabelUpdated event
                _broker.Pub<EntityLabel>(this, new MessageOptions()
                {
                    Key = "EntityLabelUpdated"
                }, updatedEntityLabel);

                // Return success
                return result.Success(updatedEntityLabel);

            }

            return result.Failed(new ActivityError("An unknown error occurred whilst attempting to create an entity / label relatinship"));
            
        }

        public async Task<IActivityResult<EntityLabel>> DeleteAsync(EntityLabel model)
        {
            
            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Publish EntityLabelDeleting event
            _broker.Pub<EntityLabel>(this, new MessageOptions()
            {
                Key = "EntityLabelDeleting"
            }, model);

            var result = new ActivityResult<EntityLabel>();
            if (await _entityLabelStore.DeleteAsync(model))
            {
                // Publish EntityLabelDeleted event
                _broker.Pub<EntityLabel>(this, new MessageOptions()
                {
                    Key = "EntityLabelDeleted"
                }, model);

                // Return success
                return result.Success();

            }

            return result.Failed(new ActivityError("An unknown error occurred whilst attempting to delete the entity / label relationship"));


        }

    }

}
