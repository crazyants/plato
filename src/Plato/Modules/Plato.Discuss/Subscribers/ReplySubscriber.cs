using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
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
        private readonly IEntityUsersStore _entityUsersStore;

        public ReplySubscriber(
            IBroker broker,
            IEntityStore<Topic> entityStore,
            IEntityReplyStore<TEntityReply> entityReplyStore,
            IEntityUsersStore entityUsersStore)
        {
            _broker = broker;
            _entityStore = entityStore;
            _entityReplyStore = entityReplyStore;
            _entityUsersStore = entityUsersStore;
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
            
            if (reply.EntityId <= 0)
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
            
            // Get last 5 unique users & total unique user count
            var users = await _entityUsersStore.QueryAsync()
                .Take(1, 5)
                .Select<EntityUserQueryParams>(q =>
                {
                    q.EntityId.Equals(entity.Id);
                })
                .OrderBy("r.CreatedDate", OrderBy.Desc)
                .ToList();

            details.LatestUsers = users?.Data;
            entity.TotalParticipants = users?.Total ?? 0;

            // Add updated data to entity
            entity.AddOrUpdate<PostDetails>(details);

            // Persist the updates
            await _entityStore.UpdateAsync(entity);

            return reply;
            
        }

        #endregion

        async Task<IEnumerable<EntityUser>> GetLastFiveUniqueUsers(int entityId)
        {
            
            var results = await _entityUsersStore.QueryAsync()
                .Take(1, 20)
                .Select<EntityUserQueryParams>(q =>
                {
                    q.EntityId.Equals(entityId);
                })
                .OrderBy("r.CreatedDate", OrderBy.Desc)
                .ToList();

            return results.Data;

            //return await _entityUsersStore.GetUniqueUsers(new EntityUserQueryParams()
            //{
            //    EntityId = entityId,
            //    PageSize = 5,
            //    Sort = EntityUserQueryParams.SortBy.CreatedDate
            //});
        }

        //async Task<IPagedResults<TEntityReply>> GetReplies(Entity entity)
        //{
        //    return await _entityReplyStore.QueryAsync()
        //        .Select<EntityReplyQueryParams>(q =>
        //        {
        //            q.EntityId.Equals(entity.Id);
        //            q.IsPrivate.False();
        //            q.IsSpam.False();
        //            q.IsDeleted.False();
        //        })
        //        .ToList();

        //}

    }

}
