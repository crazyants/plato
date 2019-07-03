using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Plato.Discuss.Categories.Follow.NotificationTypes;
using Plato.Entities.Models;
using Plato.Follows.Stores;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Notifications.Extensions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;
using Plato.Internal.Tasks.Abstractions;
using Plato.Entities.Extensions;
using Plato.Entities.Stores;
using Plato.Follows.Models;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.Categories.Follow.Subscribers
{
    public class EntitySubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {
        
        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;
        private readonly IDummyClaimsPrincipalFactory<User> _claimsPrincipalFactory;
        private readonly INotificationManager<TEntity> _notificationManager;
        private readonly IFollowStore<Follows.Models.Follow> _followStore;
        private readonly IAuthorizationService _authorizationService;
        private readonly IDeferredTaskManager _deferredTaskManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IEntityStore<TEntity> _entityStore;
        private readonly IBroker _broker;

        public EntitySubscriber(
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            IDummyClaimsPrincipalFactory<User> claimsPrincipalFactory,
            INotificationManager<TEntity> notificationManager,
            IFollowStore<Follows.Models.Follow> followStore,
            IAuthorizationService authorizationService,
            IDeferredTaskManager deferredTaskManager,
            IPlatoUserStore<User> platoUserStore,
            IEntityStore<TEntity> entityStore,
            IBroker broker)
        {
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _authorizationService = authorizationService;
            _deferredTaskManager = deferredTaskManager;
            _notificationManager = notificationManager;
            _platoUserStore = platoUserStore;
            _followStore = followStore;
            _entityStore = entityStore;
            _broker = broker;
        }

        #region "Implementation"

        public void Subscribe()
        {
            // Created
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated",
                Order = short.MaxValue
            }, async message => await EntityCreated(message.What));

            // Updated
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdated",
                Order = short.MaxValue
            }, async message => await EntityUpdated(message.What));

        }

        public void Unsubscribe()
        {
            // Created
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated",
                Order = short.MaxValue
            }, async message => await EntityCreated(message.What));

            // Updated
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdated",
                Order = short.MaxValue
            }, async message => await EntityUpdated(message.What));

        }

        #endregion

        #region "Private Methods"

        async Task<TEntity> EntityCreated(TEntity entity)
        {
            entity = await SendNotificationsForCategory(entity);
            return await SendNotificationsForAllCategories(entity);
        }

        async Task<TEntity> EntityUpdated(TEntity entity)
        {
            entity = await SendNotificationsForCategory(entity);
            return await SendNotificationsForAllCategories(entity);
        }

        Task<TEntity> SendNotificationsForCategory(TEntity entity)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // The entity is NOT posted within a specific category so no need to send notifications
            if (entity.CategoryId == 0)
            {
                return Task.FromResult(entity);
            }

            // We don't need to trigger notifications for hidden entities
            if (entity.IsHidden())
            {
                return Task.FromResult(entity);
            }
            
            // Defer notifications to first available thread pool thread
            _deferredTaskManager.AddTask(async context =>
            {

                // Follow type name
                var name = FollowTypes.Category.Name;

                // Get follow state for entity
                var state = entity.GetOrCreate<FollowState>();

                // Have notifications already been sent for the entity?
                var follow = state.FollowsSent.FirstOrDefault(f =>
                    f.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                if (follow != null)
                {
                    return;
                }
                
                // Get all follows for the category
                var follows = await _followStore.QueryAsync()
                    .Select<FollowQueryParams>(q =>
                    {
                        q.ThingId.Equals(entity.CategoryId);
                        q.Name.Equals(name);
                    })
                    .ToList();

                // Get all users for the follow
                var users = await GetUsersAsync(follows?.Data, entity);

                // No users simply return
                if (users == null)
                {
                    return;
                }

                // Send notifications
                foreach (var user in users)
                {

                    // Email notifications
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.NewTopic))
                    {
                        await _notificationManager.SendAsync(new Notification(EmailNotifications.NewTopic)
                        {
                            To = user,
                        }, entity);
                    }

                    // Web notifications
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.NewTopic))
                    {
                        await _notificationManager.SendAsync(new Notification(WebNotifications.NewTopic)
                        {
                            To = user,
                            From = new User
                            {
                                Id = entity.CreatedBy.Id,
                                UserName = entity.CreatedBy.UserName,
                                DisplayName = entity.CreatedBy.DisplayName,
                                Alias = entity.CreatedBy.Alias,
                                PhotoUrl = entity.CreatedBy.PhotoUrl,
                                PhotoColor = entity.CreatedBy.PhotoColor
                            }
                        }, entity);
                    }

                }

                // Update state
                state.AddSent(name);
                entity.AddOrUpdate(state);

                // Persist state
                await _entityStore.UpdateAsync(entity);
                
            });

            return Task.FromResult(entity);

        }

        Task<TEntity> SendNotificationsForAllCategories(TEntity entity)
        {
            
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // We don't need to trigger notifications for hidden entities
            if (entity.IsHidden())
            {
                return Task.FromResult(entity);
            }
            
            // Defer notifications to first available thread pool thread
            _deferredTaskManager.AddTask(async context =>
            {
                
                // Follow type name
                var name = FollowTypes.AllCategories.Name;

                // Get follow state for entity
                var state = entity.GetOrCreate<FollowState>();

                // Have notifications already been sent for the entity?
                var follow = state.FollowsSent.FirstOrDefault(f =>
                    f.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                if (follow != null)
                {
                    return;
                }
                
                // Get all follows
                var follows = await _followStore.QueryAsync()
                    .Select<FollowQueryParams>(q =>
                    {
                        q.Name.Equals(name);
                    })
                    .ToList();

                // Get all users for the follow
                var users = await GetUsersAsync(follows?.Data, entity);

                // No users simply return
                if (users == null)
                {
                    return;
                }

                // Send notifications
                foreach (var user in users)
                {

                    // Email notifications
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.NewTopic))
                    {
                        await _notificationManager.SendAsync(new Notification(EmailNotifications.NewTopic)
                        {
                            To = user,
                        }, entity);
                    }

                    // Web notifications
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.NewTopic))
                    {
                        await _notificationManager.SendAsync(new Notification(WebNotifications.NewTopic)
                        {
                            To = user,
                            From = new User
                            {
                                Id = entity.CreatedBy.Id,
                                UserName = entity.CreatedBy.UserName,
                                DisplayName = entity.CreatedBy.DisplayName,
                                Alias = entity.CreatedBy.Alias,
                                PhotoUrl = entity.CreatedBy.PhotoUrl,
                                PhotoColor = entity.CreatedBy.PhotoColor
                            }
                        }, entity);
                    }

                }

                // Update state
                state.AddSent(name);
                entity.AddOrUpdate(state);

                // Persist state
                await _entityStore.UpdateAsync(entity);

            });

            return Task.FromResult(entity);

        }

        // TODO: Look at centralizing within class
        async Task<IEnumerable<IUser>> GetUsersAsync(
            IEnumerable<Follows.Models.Follow> follows,
            TEntity entity)
        {

            // We always need follows to process
            if (follows == null)
            {
                return null;
            }

            // Get all users following the category
            // Exclude the author so they are not notified of there own posts
            var users = await _platoUserStore.QueryAsync()
                .Select<UserQueryParams>(q =>
                {
                    q.Id.IsIn(follows
                        .Where(f => f.CreatedUserId != entity.CreatedUserId)
                        .Select(f => f.CreatedUserId)
                        .ToArray());
                })
                .ToList();

            // No users to further process
            if (users?.Data == null)
            {
                return null;
            }
            
            // Build users reducing for permissions
            var result = new Dictionary<int, IUser>();
            foreach (var user in users.Data)
            {

                if (!result.ContainsKey(user.Id))
                {
                    result.Add(user.Id, user);
                }
                
                // If the entity is hidden but the user does
                // not have permission to view hidden entities
                if (entity.IsHidden)
                {
                    var principal = await _claimsPrincipalFactory.CreateAsync(user);
                    if (!await _authorizationService.AuthorizeAsync(principal,
                        entity.CategoryId, Permissions.ViewHiddenTopics))
                    {
                        result.Remove(user.Id);
                    }
                }

                // If we are not the entity author and the entity is private
                // ensure we have permission to view private entities
                if (user.Id != entity.CreatedUserId && entity.IsPrivate)
                {
                    var principal = await _claimsPrincipalFactory.CreateAsync(user);
                    if (!await _authorizationService.AuthorizeAsync(principal,
                        entity.CategoryId, Permissions.ViewPrivateTopics))
                    {
                        result.Remove(user.Id);
                    }
                }

                // The entity has been flagged as SPAM but the user does
                // not have permission to view entities flagged as SPAM
                if (entity.IsSpam)
                {
                    var principal = await _claimsPrincipalFactory.CreateAsync(user);
                    if (!await _authorizationService.AuthorizeAsync(principal,
                        entity.CategoryId, Permissions.ViewSpamTopics))
                    {
                        result.Remove(user.Id);
                    }
                }

                // The entity is soft deleted but the user does 
                // not have permission to view soft deleted entities
                if (entity.IsDeleted)
                {
                    var principal = await _claimsPrincipalFactory.CreateAsync(user);
                    if (!await _authorizationService.AuthorizeAsync(principal,
                        entity.CategoryId, Permissions.ViewDeletedTopics))
                    {
                        result.Remove(user.Id);
                    }
                }
                
            }

            return result.Values;

        }
        
        #endregion

    }

}
