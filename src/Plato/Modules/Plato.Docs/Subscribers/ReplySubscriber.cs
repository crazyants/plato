using System;
using System.Threading.Tasks;
using Plato.Docs.Models;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Docs.Subscribers
{

    public class DocCommentSubscriber<TEntityReply> : IBrokerSubscriber where TEntityReply : class, IEntityReply
    {
        
        private readonly IUserReputationAwarder _reputationAwarder;
        private readonly IEntityUsersStore _entityUsersStore;
        private readonly IEntityStore<Doc> _entityStore;
        private readonly IBroker _broker;

        public DocCommentSubscriber(
            IUserReputationAwarder reputationAwarder,
            IEntityUsersStore entityUsersStore,
            IEntityStore<Doc> entityStore,
            IBroker broker)
        {
            _reputationAwarder = reputationAwarder;
            _entityUsersStore = entityUsersStore;
            _entityStore = entityStore;
            _broker = broker;
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

            // Deleted
            _broker.Sub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyDeleted"
            }, async message => await EntityReplyDeleted(message.What));
            
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

            // Deleted
            _broker.Unsub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyDeleted"
            }, async message => await EntityReplyDeleted(message.What));

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

            // Award reputation for new reply
            await _reputationAwarder.AwardAsync(Reputations.NewReply, reply.CreatedUserId, "Posted a reply");

            // Update entity details
            return await EntityDetailsUpdater(reply);

        }

        async Task<TEntityReply> EntityReplyUpdated(TEntityReply reply)
        {

            if (reply == null)
            {
                throw new ArgumentNullException(nameof(reply));
            }

            // Update entity details
            return await EntityDetailsUpdater(reply);
            
        }

        async Task<TEntityReply> EntityReplyDeleted(TEntityReply reply)
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

            // Revoke awarded reputation 
            await _reputationAwarder.RevokeAsync(Reputations.NewReply, reply.CreatedUserId, "Reply deleted or hidden");

            // Return reply
            return reply;

        }
        
        async Task<TEntityReply> EntityDetailsUpdater(TEntityReply reply)
        {
            
            // We need an entity to update
            if (reply.EntityId <= 0)
            {
                return reply;
            }

            // Ensure the entity exists
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);
            if (entity == null)
            {
                return reply;
            }
            
            // Get entity details to update
            var details = entity.GetOrCreate<DocDetails>();

            // Get last 5 unique users & total unique user count
            var users = await _entityUsersStore.QueryAsync()
                .Take(1, 5)
                .Select<EntityUserQueryParams>(q =>
                {
                    q.EntityId.Equals(entity.Id);
                    q.HidePrivate.True();
                    q.HideDeleted.True();
                    q.HideSpam.True();
                })
                .OrderBy("r.CreatedDate", OrderBy.Desc)
                .ToList();

            details.LatestUsers = users?.Data;
            entity.TotalParticipants = users?.Total ?? 0;

            // Add updated data to entity
            entity.AddOrUpdate<DocDetails>(details);

            // Persist the updates
            await _entityStore.UpdateAsync(entity);

            // Return our reply
            return reply;

        }
        
        #endregion

    }

}
