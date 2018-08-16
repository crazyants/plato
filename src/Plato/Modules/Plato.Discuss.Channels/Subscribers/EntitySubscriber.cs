using System.Threading.Tasks;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Entities.Models;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Discuss.Channels.Subscribers
{

    /// <summary>
    /// Updates category meta data whenever an entity is created.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class EntitySubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {

        private readonly IBroker _broker;
        private readonly ICategoryStore<Channel> _channelStore;

        public EntitySubscriber(
            IBroker broker,
            ICategoryStore<Channel> channelStore)
        {
            _broker = broker;
            _channelStore = channelStore;
        }

        public void Subscribe()
        {
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));
        }

        public void Unsubscribe()
        {
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));

        }

        async Task EntityCreated(TEntity entity)
        {
          
            // No need to update cateogry for private entities
            if (entity.IsPrivate)
            {
                return;
            }

            // No need to update cateogry for soft deleted entities
            if (entity.IsDeleted)
            {
                return;
            }

            // No need to update cateogry for entities flagged as spam
            if (entity.IsSpam)
            {
                return;
            }


            // Ensure we have a categoryId for the newly created entity
            if (entity.CategoryId <= 0)
            {
                return;
            }
            
            // Ensure we found the category
            var channel = await _channelStore.GetByIdAsync(entity.CategoryId);
            if (channel == null)
            {
                return;
            }

            // Update details with latest entity
            var details = channel.GetOrCreate<ChannelDetails>();
            details.LatestTopic.Id = entity.Id;
            details.LatestTopic.CreatedUserId = entity.CreatedUserId;
            details.LatestTopic.CreatedDate = entity.CreatedDate;
            channel.AddOrUpdate<ChannelDetails>(details);

            // Save the update details 
            await _channelStore.UpdateAsync(channel);
            
        }

        public void Dispose()
        {
            Unsubscribe();
        }
    }

}
