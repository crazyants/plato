using System;
using System.Threading.Tasks;
using Plato.Articles.Models;
using Plato.Entities.Extensions;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Reputations.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Articles.Subscribers
{
    
    public class ArticleSubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {

        private readonly IEntityRepository<TEntity> _entityRepository;
        private readonly IUserReputationAwarder _reputationAwarder;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IBroker _broker;

        public ArticleSubscriber(
            IEntityRepository<TEntity> entityRepository,
            IUserReputationAwarder reputationAwarder,
            IPlatoUserStore<User> platoUserStore,
            IBroker broker)
        {
            _reputationAwarder = reputationAwarder;
            _entityRepository = entityRepository;
            _platoUserStore = platoUserStore;
            _broker = broker;
        }

        #region "Implementation"

        public void Subscribe()
        {
            // Created
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));

            // Updating
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdating"
            }, async message => await EntityUpdating(message.What));

            // Deleted
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityDeleted"
            }, async message => await EntityDeleted(message.What));
            
        }

        public void Unsubscribe()
        {

            // Created
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));

            // Updating
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdating"
            }, async message => await EntityUpdating(message.What));

            // Deleted
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityDeleted"
            }, async message => await EntityDeleted(message.What));


        }

        #endregion

        #region "Private Methods"

        async Task<TEntity> EntityCreated(TEntity entity)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (entity.IsHidden())
            {
                return entity;
            }

            // Award reputation
            if (entity.CreatedUserId > 0)
            {
                await _reputationAwarder.AwardAsync(Reputations.NewArticle, entity.CreatedUserId, "Created an article");
            }

            // Return
            return entity;

        }

        async Task<TEntity> EntityUpdating(TEntity entity)
        {

            // Award reputation if the entity becomes visible
            entity = await AwardReputationAsync(entity);

            // Populate contributor meta data
            entity = await BuildContributorsAsync(entity);
            
            // Return
            return entity;

        }

        async Task<TEntity> EntityDeleted(TEntity entity)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (entity.IsHidden())
            {
                return entity;
            }

            // Revoke awarded reputation 
            if (entity.CreatedUserId > 0)
            {
                await _reputationAwarder.RevokeAsync(Reputations.NewArticle, entity.CreatedUserId,
                    "Article deleted or hidden");
            }

            return entity;


        }

        async Task<TEntity> AwardReputationAsync(TEntity entity)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Get existing entity before any changes
            var existingEntity = await _entityRepository.SelectByIdAsync(entity.Id);

            // We need an existing entity
            if (existingEntity == null)
            {
                return entity;
            }

            // Entity has been hidden
            if (entity.IsHidden())
            {
                // If the existing entity was not already hidden revoke reputation
                if (!existingEntity.IsHidden())
                {
                    if (entity.CreatedUserId > 0)
                    {
                        await _reputationAwarder.RevokeAsync(Reputations.NewArticle, entity.CreatedUserId,
                            "Article deleted or hidden");
                    }
                }
            }
            else
            {
                // If the existing entity was already hidden award reputation
                if (existingEntity.IsHidden())
                {
                    if (entity.CreatedUserId > 0)
                    {
                        await _reputationAwarder.AwardAsync(Reputations.NewArticle, entity.CreatedUserId,
                            "Article approved or made visible");
                    }
                }
            }

            return entity;


        }

        async Task<TEntity> BuildContributorsAsync(TEntity entity)
        {

            // We always need a contributor to add
            if (entity.ModifiedUserId <= 0)
            {
                return entity;
            }

            // Get user modifying entity
            var user = await _platoUserStore.GetByIdAsync(entity.ModifiedUserId);

            // We always need a contributor to add
            if (user == null)
            {
                return entity;
            }

            // Get entity details to update
            var details = entity.GetOrCreate<ArticleDetails>();

            // No need to add the contributor more than once
            var exists = false;
            foreach (var contributor in details.Contributors)
            {
                if (contributor.Id == user.Id)
                {

                    // Indicate contributor already exists
                    exists = true;

                    // Update contributor details as they may change
                    contributor.Id = user.Id;
                    contributor.UserName = user.UserName;
                    contributor.DisplayName = user.DisplayName;
                    contributor.Alias = user.Alias;
                    contributor.PhotoColor = user.PhotoColor;
                    contributor.PhotoUrl = user.PhotoUrl;
                    contributor.Signature = "";
                    contributor.SignatureHtml = "";

                    // update contribution data
                    contributor.ContributionCount += 1;
                    contributor.ContributionDate = DateTimeOffset.UtcNow;

                }

            }

            // If the contributor does not exist add them
            if (!exists)
            {
                details.Contributors.Add(new EntityContributor()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    DisplayName = user.DisplayName,
                    Alias = user.Alias,
                    PhotoColor = user.PhotoColor,
                    PhotoUrl = user.PhotoUrl,
                    Signature = "",
                    SignatureHtml = "",
                    ContributionCount = 1,
                    ContributionDate = DateTimeOffset.UtcNow
                });
            }

            // Add updated data to entity
            entity.AddOrUpdate<ArticleDetails>(details);

            // Return updated entity
            return entity;

        }

        #endregion

    }

}
