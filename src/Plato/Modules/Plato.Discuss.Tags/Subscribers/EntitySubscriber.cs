using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Internal.Messaging.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Services;
using Plato.Tags.Stores;
using Plato.Discuss.Tags.Models;

namespace Plato.Discuss.Tags.Subscribers
{

    public class EntitySubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {

        private readonly ITagOccurrencesUpdater<Tag> _tagOccurrencesUpdater;
        private readonly IEntityTagStore<EntityTag> _entityTagStore;
        private readonly IBroker _broker;
    
        public EntitySubscriber(
            ITagOccurrencesUpdater<Tag> tagOccurrencesUpdater,
            IEntityTagStore<EntityTag> entityTagStore,
            IBroker broker)
        {
            _tagOccurrencesUpdater = tagOccurrencesUpdater;
            _entityTagStore = entityTagStore;
            _broker = broker;
        }

        #region "Implementation"

        public void Subscribe()
        {
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdated"
            }, async message => await EntityUpdated(message.What));
        }

        public void Unsubscribe()
        {
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdated"
            }, async message => await EntityUpdated(message.What));
        }

        #endregion

        #region "Private Methods"
        
        async Task<TEntity> EntityUpdated(TEntity entity)
        {

            // Get all tags for entity
            var tags = await _entityTagStore.QueryAsync()
                .Select<EntityTagQueryParams>(q =>
                {
                    q.EntityId.Equals(entity.Id);
                })
                .ToList();

            // No tags for entity just continue
            if (tags?.Data == null)
            {
                return entity;
            }

            // Update counts for all tags associated with entity
            foreach (var entityTag in tags.Data)
            {
                await _tagOccurrencesUpdater.UpdateAsync(entityTag.ConvertToType<Tag>());
            }
            
            // return 
            return entity;

        }

        #endregion

    }

}
