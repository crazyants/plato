using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Docs.Models;
using Plato.Entities.Extensions;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Entities.Stores;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Reputations.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Docs.Subscribers
{
    
    public class DocSubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {

        private readonly IEntityRepository<TEntity> _entityRepository;
        private readonly IUserReputationAwarder _reputationAwarder;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IBroker _broker;

        public DocSubscriber(
            IUserReputationAwarder reputationAwarder,
            IEntityRepository<TEntity> entityRepository,
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

            // Creating
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreating"
            }, async message => await EntityCreating(message.What));
            
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
            
            //// Updated
            //_broker.Sub<TEntity>(new MessageOptions()
            //{
            //    Key = "EntityUpdated"
            //}, async message => await EntityUpdated(message.What));
            
        }

        public void Unsubscribe()
        {

            // Creating
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreating"
            }, async message => await EntityCreating(message.What));

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

            //// Updated
            //_broker.Unsub<TEntity>(new MessageOptions()
            //{
            //    Key = "EntityUpdated"
            //}, async message => await EntityUpdated(message.What));

        }

        #endregion

        #region "Private Methods"

        async Task<TEntity> EntityCreating(TEntity entity)
        {
            // Get the next available sort order for new entries
            if (entity.SortOrder == 0)
            {
                entity.SortOrder = await GetNextAvailableSortOrder(entity);
            }

            return entity;

        }
        
        async Task<TEntity> EntityCreated(TEntity entity)
        {

            if (entity.IsHidden())
            {
                return entity;
            }

            // Award reputation
            if (entity.CreatedUserId > 0)
            {
                await _reputationAwarder.AwardAsync(Reputations.NewTopic, entity.CreatedUserId, "Topic posted");
            }
           
            // Return
            return entity;

        }

        async Task<TEntity> EntityUpdating(TEntity entity)
        {
            
            // Populate sort order
            entity = await BuildSortOrderAsync(entity);

            // Populate contributor meta data
            entity = await BuildContributorsAsync(entity);
            
            // Return
            return entity;

        }

        //async Task<TEntity> EntityUpdated(TEntity entity)
        //{

        //    // We always need a contributor to add
        //    if (entity.ModifiedUserId <= 0)
        //    {
        //        return entity;
        //    }

        //    // Get entity details to update
        //    var details = entity.GetOrCreate<DocDetails>();

        //    // Get user modifying entity
        //    var user = await _platoUserStore.GetByIdAsync(entity.ModifiedUserId);

        //    // We always need a contributor to add
        //    if (user == null)
        //    {
        //        return entity;
        //    }

        //    // No need to add the contributor more than once
        //    var exists = false;
        //    foreach (var contributor in details.Contributors)
        //    {
        //        if (contributor.Id == user.Id)
        //        {
        //            exists = true;
        //            contributor.Contributions.Add(new Contribution(DateTimeOffset.UtcNow));
        //        }
        //    }

        //    if (!exists)
        //    {
        //        // Add our contributor
        //        details.Contributors.Add(new EntityContributor(user)
        //        {
        //            Contributions = new List<Contribution>()
        //            {
        //                new Contribution(DateTimeOffset.UtcNow)
        //            }
        //        });
        //    }

        //    // Add updated data to entity
        //    entity.AddOrUpdate<DocDetails>(details);

        //    // Persist the updates
        //    return await _entityStore.UpdateAsync(entity);

        //}

        // ------------

        async Task<TEntity> BuildSortOrderAsync(TEntity entity)
        {

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
                        await _reputationAwarder.RevokeAsync(Reputations.NewTopic, entity.CreatedUserId, "Topic deleted or hidden");
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
                        await _reputationAwarder.AwardAsync(Reputations.NewTopic, entity.CreatedUserId, "Topic approved or made visible");
                    }
                }
            }

            // If the parent changes ensure we update the sort order
            if (entity.ParentId != existingEntity.ParentId)
            {
                entity.SortOrder = await GetNextAvailableSortOrder(entity);
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
            var details = entity.GetOrCreate<DocDetails>();

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
            entity.AddOrUpdate<DocDetails>(details);

            // Return updated entity
            return entity;

        }

        async Task<int> GetNextAvailableSortOrder(TEntity model)
        {

            var sortOrder = 0;
            var entities = await _entityRepository
                .SelectByFeatureIdAsync(model.FeatureId);

            if (entities != null)
            {
                var orderedEntities = entities
                    .Where(c => c.CategoryId == model.CategoryId && c.ParentId == model.ParentId)
                    .OrderBy(o => o.SortOrder);
                foreach (var entity in orderedEntities)
                {
                    sortOrder = entity.SortOrder;
                }
            }

            return sortOrder + 1;

        }
        

        #endregion

    }

}
