using System.Linq;
using System.Threading.Tasks;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Models;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
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
        private readonly ICategoryManager<Channel> _channelManager;
        private readonly IEntityStore<Topic> _topicStore;

        public EntitySubscriber(
            IBroker broker,
            ICategoryStore<Channel> channelStore,
            ICategoryManager<Channel> channelManager,
            IEntityStore<Topic> topicStore)
        {
            _broker = broker;
            _channelStore = channelStore;
            _channelManager = channelManager;
            _topicStore = topicStore;
        }

        #region "Implementation"

        public void Subscribe()
        {
            // Created
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));

            // Updated
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdated"
            }, async message => await EntityCreated(message.What));
        }

        public void Unsubscribe()
        {
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));
        }
        
        #endregion

        #region "Private Methods"

        async Task<TEntity> EntityCreated(TEntity entity)
        {

            // No need to update cateogry for private entities
            if (entity.IsPrivate)
            {
                return entity;
            }

            // No need to update cateogry for soft deleted entities
            if (entity.IsDeleted)
            {
                return entity;
            }

            // No need to update cateogry for entities flagged as spam
            if (entity.IsSpam)
            {
                return entity;
            }

            // Ensure we have a categoryId for the newly created entity
            if (entity.CategoryId <= 0)
            {
                return entity;
            }

            // Ensure we found the category
            var channel = await _channelStore.GetByIdAsync(entity.CategoryId);
            if (channel == null)
            {
                return entity;
            }

            // Get current channel and all parent channels
            var parents = await _channelStore.GetParentsByIdAsync(channel.Id);

            // Update details within current and all parents
            foreach (var parent in parents)
            {
                
                // Get topic details for current channel
                var topics = await _topicStore.QueryAsync()
                    .Take(1, 1) // we only need the latest topic
                    .Select<EntityQueryParams>(q =>
                    {

                        // If the channel has children include all child topics
                        if (parent.Children.Any())
                        {
                            q.CategoryId.IsIn(parent.Children.Select(c => c.Id).ToArray());
                        }
                        else
                        {
                            // Get topics for current channel
                            q.CategoryId.Equals(parent.Id);
                        }
                        
                        q.HideSpam.True();
                        q.HidePrivate.True();
                        q.HideDeleted.True();
                    })
                    .OrderBy("LastReplyDate", OrderBy.Desc)
                    .ToList();

                // Details we'll store within the channel details
                var totalTopics = 0;
                Topic latestTopic = null;
                if (topics?.Data != null)
                {
                    totalTopics = topics.Total;
                    latestTopic = topics.Data[0];
                }

                // Update channel details with latest entity details
                var details = parent.GetOrCreate<ChannelDetails>();
                details.TotalTopics = totalTopics;
                if (latestTopic != null)
                {
                    details.LastPost.EntityId = latestTopic.Id;
                    details.LastPost.CreatedBy = latestTopic.CreatedBy;
                    details.LastPost.CreatedDate = latestTopic.CreatedDate;
                }
                parent.AddOrUpdate<ChannelDetails>(details);

                // Save the updated details 
                await _channelManager.UpdateAsync(parent);

            }

            return entity;

        }
  
        #endregion

    }

}
