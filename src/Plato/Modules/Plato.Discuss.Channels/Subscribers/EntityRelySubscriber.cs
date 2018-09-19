using System.Threading.Tasks;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Models;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Discuss.Channels.Subscribers
{

    /// <summary>
    /// Updates category meta data whenever an entity reply is created.
    /// </summary>
    /// <typeparam name="TEntityReply"></typeparam>
    public class EntityReplySubscriber<TEntityReply> : IBrokerSubscriber where TEntityReply : class, IEntityReply
    {

        private readonly IBroker _broker;
        private readonly ICategoryStore<Channel> _channelStore;
        private readonly ICategoryManager<Channel> _channelManager;
        private readonly IEntityStore<Topic> _topicStore;

        public EntityReplySubscriber(
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
            _broker.Sub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyCreated"
            }, async message => await EntityReplyCreated(message.What));
        }

        public void Unsubscribe()
        {
            _broker.Unsub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyCreated"
            }, async message => await EntityReplyCreated(message.What));
        }
        
        public void Dispose()
        {
            Unsubscribe();
        }

        #endregion

        #region "Private Methods"

        async Task EntityReplyCreated(TEntityReply reply)
        {

            // No need to update cateogry for private entities
            if (reply.IsPrivate)
            {
                return;
            }

            // No need to update cateogry for soft deleted entities
            if (reply.IsDeleted)
            {
                return;
            }

            // No need to update cateogry for entities flagged as spam
            if (reply.IsSpam)
            {
                return;
            }

            // Get the entity we are replying to
            var entity = await _topicStore.GetByIdAsync(reply.EntityId);
            if (entity == null)
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

            //Get current channel and all parent channels
            var parents = await _channelStore.GetParentsByIdAsync(channel.Id);

            // Update details within current and all parents
            foreach (var parent in parents)
            {

                // Update details with latest entity details
                var details = parent.GetOrCreate<ChannelDetails>();
                details.TotalReplies = details.TotalReplies + 1;
                details.LastPost.EntityId = reply.EntityId;
                details.LastPost.EntityReplyId = reply.Id;
                details.LastPost.CreatedUserId = reply.CreatedUserId;
                details.LastPost.CreatedDate = reply.CreatedDate;
                parent.AddOrUpdate<ChannelDetails>(details);

                // Save the updated details 
                await _channelManager.UpdateAsync(parent);

            }

        }
        
        #endregion

    }

}
