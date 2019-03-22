using System;
using System.Threading.Tasks;
using Plato.Questions.Models;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Questions.Subscribers
{

    public class CommentSubscriber<TEntityReply> : IBrokerSubscriber where TEntityReply : class, IEntityReply
    {

        private readonly IBroker _broker;
        private readonly IEntityStore<Question> _entityStore;
        private readonly IEntityReplyStore<TEntityReply> _entityReplyStore;
        private readonly IEntityUsersStore _entityUsersStore;

        public CommentSubscriber(
            IBroker broker,
            IEntityStore<Question> entityStore,
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
        
        #endregion

        #region "Private Methods"

        private async Task<TEntityReply> EntityDetailsUpdater(TEntityReply reply)
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
            var details = entity.GetOrCreate<QuestionDetails>();

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
            entity.AddOrUpdate<QuestionDetails>(details);

            // Persist the updates
            await _entityStore.UpdateAsync(entity);

            // Return our reply
            return reply;

        }
        
        #endregion

    }

}
