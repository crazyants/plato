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

            // Created
            _broker.Sub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyCreated"
            }, async message => await EntityReplyCreated(message.What));
           
            // Updated
            _broker.Sub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyUpdated"
            }, async message => await EntityReplyUpdated(message.What));


        }

        public void Unsubscribe()
        {

            // Created
            _broker.Unsub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyCreated"
            }, async message => await EntityReplyCreated(message.What));

            // Updated
            _broker.Unsub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyUpdated"
            }, async message => await EntityReplyUpdated(message.What));
            
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
            
            if (reply.IsHidden)
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

            // Update entity details
            return await UpdateEntityDetailsAsync(reply);

        }

        async Task<TEntityReply> EntityReplyUpdated(TEntityReply reply)
        {

            if (reply == null)
            {
                throw new ArgumentNullException(nameof(reply));
            }

            // Update entity details
            return await UpdateEntityDetailsAsync(reply);

        }
        
        async Task<TEntityReply> UpdateEntityDetailsAsync(TEntityReply reply)
        {

            // Get entity
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);
            if (entity == null)
            {
                return reply;
            }

            // Get reply details
            TEntityReply lastReply = null;
            int totalReplies = 0, totalParticipants = 0;
            var replies = await GetReplies(entity);
            if (replies?.Data != null)
            {
                totalReplies = replies.Total;
                totalParticipants = GetTotalUniqueParticipantCount(replies.Data);
                lastReply = replies.Data[0];
            }
          
            // Update last reply 
            entity.TotalReplies = totalReplies;
            entity.TotalParticipants = totalParticipants;
            entity.LastReplyId = lastReply?.Id ?? 0;
            entity.LastReplyUserId = lastReply?.CreatedUserId ?? 0;
            entity.LastReplyDate = lastReply?.CreatedDate ?? null;

            // Persist the updates
            await _entityStore.UpdateAsync(entity);

            return reply;

        }

        int GetTotalUniqueParticipantCount(IEnumerable<TEntityReply> replies)
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
                    q.HideHidden.True();
                    q.HideSpam.True();
                    q.HideDeleted.True();
                })
                .OrderBy("CreatedDate", OrderBy.Desc)
                .ToList();
        }

        #endregion

    }

}
