using System;
using System.Threading.Tasks;
using Plato.Articles.Categories.Services;
using Plato.Articles.Models;
using Plato.Categories.Models;
using Plato.Entities.Stores;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Articles.Categories.Subscribers
{
 
    /// <summary>
    /// Updates category meta data whenever an entity & category relationship is created or deleted. 
    /// </summary>
    public class EntityCategorySubscriber : IBrokerSubscriber
    {
        
        private readonly ICategoryDetailsUpdater _categoryDetailsUpdater;
        private readonly IEntityStore<Article> _entityStore;
        private readonly IFeatureFacade _featureFacade;
        private readonly IBroker _broker;
  
        public EntityCategorySubscriber(
            ICategoryDetailsUpdater categoryDetailsUpdater,
            IEntityStore<Article> entityStore,
            IFeatureFacade featureFacade,
            IBroker broker)
        {
            
            _categoryDetailsUpdater = categoryDetailsUpdater;
            _featureFacade = featureFacade;
            _entityStore = entityStore;
            _broker = broker;
        }

        #region "Implementation"

        public void Subscribe()
        {
            // Created
            _broker.Sub<EntityCategory>(new MessageOptions()
            {
                Key = "EntityCategoryCreated"
            }, async message => await EntityCategoryCreated(message.What));

            // Deleted
            _broker.Sub<EntityCategory>(new MessageOptions()
            {
                Key = "EntityCategoryDeleted"
            }, async message => await EntityCategoryDeleted(message.What));
        }

        public void Unsubscribe()
        {

            // Created
            _broker.Unsub<EntityCategory>(new MessageOptions()
            {
                Key = "EntityCategoryCreated"
            }, async message => await EntityCategoryCreated(message.What));

            // Deleted
            _broker.Unsub<EntityCategory>(new MessageOptions()
            {
                Key = "EntityCategoryDeleted"
            }, async message => await EntityCategoryDeleted(message.What));

        }

        #endregion

        #region "Private Methods"

        async Task<EntityCategory> EntityCategoryCreated(EntityCategory entityCategory)
        {

            if (entityCategory == null)
            {
                throw new ArgumentNullException(nameof(entityCategory));
            }

            // Get entity
            var entity = await _entityStore.GetByIdAsync(entityCategory.EntityId);

            // Ensure entity exists
            if (entity == null)
            {
                return entityCategory;
            }

            // Get feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Articles");

            // Ensure we found the feature
            if (feature == null)
            {
                return entityCategory;
            }

            // Ensure we are dealing with an entity from our current feature
            if (entity.FeatureId != feature.Id)
            {
                return entityCategory;
            }

            // Update category details
            await _categoryDetailsUpdater.UpdateAsync(entityCategory.CategoryId);

            // Return
            return entityCategory;

        }
        
        async Task<EntityCategory> EntityCategoryDeleted(EntityCategory entityCategory)
        {

            if (entityCategory == null)
            {
                throw new ArgumentNullException(nameof(entityCategory));
            }
            
            // Get entity
            var entity = await _entityStore.GetByIdAsync(entityCategory.EntityId);

            // Ensure entity exists
            if (entity == null)
            {
                return entityCategory;
            }

            // Get feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Articles");

            // Ensure we found the feature
            if (feature == null)
            {
                return entityCategory;
            }

            // Ensure we are dealing with an entity from our current feature
            if (entity.FeatureId != feature.Id)
            {
                return entityCategory;
            }
            
            // Update category details
            await _categoryDetailsUpdater.UpdateAsync(entityCategory.CategoryId);

            // Return
            return entityCategory;

        }

        #endregion

    }

}
