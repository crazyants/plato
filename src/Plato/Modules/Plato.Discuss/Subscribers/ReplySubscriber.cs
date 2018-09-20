using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Discuss.Subscribers
{
    public class ReplySubscriber<TEntityReply> : IBrokerSubscriber where TEntityReply : class, IEntityReply
    {

        private readonly IBroker _broker;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IEntityReplyStore<TEntityReply> _entityReplyStore;

        public ReplySubscriber(
            IBroker broker,
            IEntityStore<Topic> entityStore,
            IEntityReplyStore<TEntityReply> entityReplyStore)
        {
            _broker = broker;
            _entityStore = entityStore;
            _entityReplyStore = entityReplyStore;
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

        async Task<TEntityReply> EntityReplyCreated(TEntityReply reply)
        {

            if (reply == null)
            {
                throw new ArgumentNullException(nameof(reply));
            }
            
            if (reply.IsPrivate)
            {
                return reply;
            }

            if (reply.IsDeleted)
            {
                return reply;
            }

            if (reply.IsSpam)
            {
                return reply;
            }

            var entity = await _entityStore.GetByIdAsync(reply.EntityId);
            if (entity == null)
            {
                return reply;
            }
            
            // Get entity details to update
            var details = entity.GetOrCreate<PostDetails>();
            
            // Get all replies
            var replies = await GetReplies(entity);
            if (replies?.Data != null)
            {

                // Store details about the last 5 replys with the entity
                const int max = 5;
                var added = new List<int>();
                var simpleReplies = new List<SimpleReply>();
                foreach (var latestReply in replies.Data)
                {
                    if (!added.Contains(latestReply.CreatedUserId))
                    {
                        added.Add(latestReply.CreatedUserId);
                        simpleReplies.Add(new SimpleReply()
                        {
                            Id = latestReply.Id,
                            CreatedBy = latestReply.CreatedBy,
                            CreatedDate = latestReply.CreatedDate
                        });
                    }
                    if (added.Count >= max)
                    {
                        break;
                    }
                }
                details.LatestReplies = simpleReplies;
            }

            details.LatestReply.Id = reply.Id;
            details.LatestReply.CreatedBy = reply.CreatedBy;
            details.LatestReply.CreatedDate = reply.CreatedDate;

            // Add updated data to entity
            entity.AddOrUpdate<PostDetails>(details);

            // Persist the updates
            await _entityStore.UpdateAsync(entity);

            return reply;
            
        }

        #endregion
        
        async Task<IPagedResults<TEntityReply>> GetReplies(Entity entity)
        {

            return await _entityReplyStore.QueryAsync()
                .Select<EntityReplyQueryParams>(q =>
                {
                    q.EntityId.Equals(entity.Id);
                    q.IsPrivate.False();
                    q.IsSpam.False();
                    q.IsDeleted.False();
                })
                .ToList();

        }

    }

}
