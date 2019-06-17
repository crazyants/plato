using System.Linq;
using System.Threading.Tasks;
using Plato.Entities.Extensions;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Docs.Subscribers
{
    
    public class DocSubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {

        private readonly IEntityRepository<TEntity> _entityRepository;
        private readonly IUserReputationAwarder _reputationAwarder;
        private readonly IBroker _broker;

        public DocSubscriber(
            IUserReputationAwarder reputationAwarder,
            IEntityRepository<TEntity> entityRepository,
            IBroker broker)
        {
            _reputationAwarder = reputationAwarder;
            _entityRepository = entityRepository;
            _broker = broker;
        }

        #region "Implementation"

        public void Subscribe()
        {

            // Creating
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreating"
            }, async message => await EntityCreating(message.What));
            
            // Created
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));

            // Updating
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdating"
            }, async message => await EntityUpdating(message.What));

        }

        public void Unsubscribe()
        {

            // Creating
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreating"
            }, async message => await EntityCreating(message.What));

            // Created
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));

            // Updating
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdating"
            }, async message => await EntityUpdating(message.What));

        }

        #endregion

        #region "Private Methods"
        
        async Task<TEntity> EntityCreating(TEntity entity)
        {
            // Get the next available sort order for new entries
            if (entity.SortOrder == 0)
            {
                entity.SortOrder = await GetNextAvailableSortOrder(entity);
            }

            return entity;

        }
        
        async Task<TEntity> EntityCreated(TEntity entity)
        {

            if (entity.IsHidden())
            {
                return entity;
            }

            // Award reputation
            if (entity.CreatedUserId > 0)
            {
                await _reputationAwarder.AwardAsync(Reputations.NewTopic, entity.CreatedUserId, "Topic posted");
            }
           
            // Return
            return entity;

        }

        async Task<TEntity> EntityUpdating(TEntity entity)
        {

            // Get existing entity before any changes
            var existingEntity = await _entityRepository.SelectByIdAsync(entity.Id);

            // We need an existing entity
            if (existingEntity == null)
            {
                return entity;
            }

            // Entity has been hidden
            if (entity.IsHidden())
            {
                // If the existing entity was not already hidden revoke reputation
                if (!existingEntity.IsHidden())
                { 
                    if (entity.CreatedUserId > 0)
                    {
                        await _reputationAwarder.RevokeAsync(Reputations.NewTopic, entity.CreatedUserId, "Topic deleted or hidden");
                    }
                }
            }
            else
            {
                // If the existing entity was already hidden award reputation
                if (existingEntity.IsHidden())
                {
                    if (entity.CreatedUserId > 0)
                    {
                        await _reputationAwarder.AwardAsync(Reputations.NewTopic, entity.CreatedUserId, "Topic approved or made visible");
                    }
                }
            }
            
            // If the parent changes ensure we update the sort order
            if (entity.ParentId != existingEntity.ParentId)
            {
                entity.SortOrder = await GetNextAvailableSortOrder(entity);
            }
         
            // Return
            return entity;

        }
        
        async Task<int> GetNextAvailableSortOrder(TEntity model)
        {

            var sortOrder = 0;
            var entities = await _entityRepository
                .SelectByFeatureIdAsync(model.FeatureId);

            if (entities != null)
            {
                var orderedEntities = entities
                    .Where(c => c.CategoryId == model.CategoryId && c.ParentId == model.ParentId)
                    .OrderBy(o => o.SortOrder);
                foreach (var entity in orderedEntities)
                {
                    sortOrder = entity.SortOrder;
                }
            }

            return sortOrder + 1;

        }
        
        #endregion

    }

}
