using System;
using System.Threading.Tasks;
using Plato.Articles.Models;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Articles.Subscribers
{

    public class CommentSubscriber<TEntityReply> : IBrokerSubscriber where TEntityReply : class, IEntityReply
    {

        private readonly IBroker _broker;
        private readonly IEntityStore<Article> _entityStore;
        private readonly IEntityUsersStore _entityUsersStore;
        private readonly IUserReputationAwarder _reputationAwarder;

        public CommentSubscriber(
            IBroker broker,
            IEntityStore<Article> entityStore,
            IEntityUsersStore entityUsersStore,
            IUserReputationAwarder reputationAwarder)
        {
            _broker = broker;
            _entityStore = entityStore;
            _entityUsersStore = entityUsersStore;
            _reputationAwarder = reputationAwarder;
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

            // Award reputation for new reply
            if (reply.CreatedUserId > 0)
            {
                await _reputationAwarder.AwardAsync(Reputations.NewComment, reply.CreatedUserId,
                    "Posted an article comment");
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

        async Task<TEntityReply> EntityReplyDeleted(TEntityReply reply)
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

            // Revoke awarded reputation 
            if (reply.CreatedUserId > 0)
            {
                await _reputationAwarder.RevokeAsync(Reputations.NewComment, reply.CreatedUserId,
                    "Comment deleted or hidden");
            }

            // Return reply
            return reply;

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
            var details = entity.GetOrCreate<ArticleDetails>();

            // Get last 5 unique users & total unique user count
            var users = await _entityUsersStore.QueryAsync()
                .Take(1, 5)
                .Select<EntityUserQueryParams>(q =>
                {
                    q.EntityId.Equals(entity.Id);
                    q.HideHidden.True();
                    q.HideDeleted.True();
                    q.HideSpam.True();
                })
                .OrderBy("r.CreatedDate", OrderBy.Desc)
                .ToList();

            details.LatestUsers = users?.Data;
            entity.TotalParticipants = users?.Total ?? 0;

            // Add updated data to entity
            entity.AddOrUpdate<ArticleDetails>(details);

            // Persist the updates
            await _entityStore.UpdateAsync(entity);

            // Return our reply
            return reply;

        }
        
        #endregion

    }

}
