using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Entities.Subscribers
{
    public class EntityReplySubscriber<TEntityReply> : IBrokerSubscriber where TEntityReply : class, IEntityReply
    {

        private readonly IBroker _broker;
        private readonly IEntityStore<Entity> _entityStore;
        private readonly IEntityReplyStore<TEntityReply> _entityReplyStore;

        public EntityReplySubscriber(
            IBroker broker,
            IEntityStore<Entity> entityStore,
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

            // Update totals
            var replies = await GetReplies(entity);
            if (replies?.Data != null)
            {
                entity.TotalReplies = replies.Total;
                entity.TotalParticipants = GetTotalUniqueParicipantCount(replies.Data);
            }

            // Update last reply 
            entity.LastReplyId = reply.Id;
            entity.LastReplyUserId = reply.CreatedUserId;
            entity.LastReplyDate = reply.CreatedDate;
            
            // Persist the updates
            await _entityStore.UpdateAsync(entity);
            
            // Return reply
            return reply;

        }

        #endregion

        int GetTotalUniqueParicipantCount(IEnumerable<TEntityReply> replies)
        {

            var output = 0;
            var added = new List<int>();
            foreach (var reply in replies)
            {
                if (!added.Contains(reply.CreatedUserId))
                {
                    added.Add(reply.CreatedUserId);
                    output++;
                }
            }

            return output;

        }

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
