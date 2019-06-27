using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Internal.Messaging.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Services;
using Plato.Tags.Stores;
using Plato.Discuss.Tags.Models;

namespace Plato.Discuss.Tags.Subscribers
{

    public class EntityReplySubscriber<TEntityReply> : IBrokerSubscriber where TEntityReply : class, IEntityReply
    {

        private readonly ITagOccurrencesUpdater<Tag> _tagOccurrencesUpdater;
        private readonly IEntityTagStore<EntityTag> _entityTagStore;
        private readonly IBroker _broker;
    
        public EntityReplySubscriber(
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
            _broker.Sub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyUpdated"
            }, async message => await EntityReplyUpdated(message.What));
        }

        public void Unsubscribe()
        {
            _broker.Unsub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyUpdated"
            }, async message => await EntityReplyUpdated(message.What));
        }

        #endregion

        #region "Private Methods"
        
        async Task<TEntityReply> EntityReplyUpdated(TEntityReply reply)
        {

            // Get all tags for reply
            var replyTags = await _entityTagStore.QueryAsync()
                .Select<EntityTagQueryParams>(q =>
                {
                    q.EntityReplyId.Equals(reply.Id);
                })
                .ToList();

            // No tags for reply just continue
            if (replyTags?.Data == null)
            {
                return reply;
            }
            
            // Update counts for all tags associated with reply
            foreach (var replyTag in replyTags.Data)
            {
                await _tagOccurrencesUpdater.UpdateAsync(replyTag.ConvertToType<Tag>());
            }
            
            // return 
            return reply;

        }

        #endregion

    }

}
