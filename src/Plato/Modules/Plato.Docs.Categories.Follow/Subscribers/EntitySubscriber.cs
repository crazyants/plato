using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Plato.Docs.Categories.Follow.NotificationTypes;
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
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Docs.Categories.Follow.Subscribers
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
        private readonly IContextFacade _contextFacade;
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
            IContextFacade contextFacade,
            IBroker broker)
        {
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _authorizationService = authorizationService;
            _deferredTaskManager = deferredTaskManager;
            _notificationManager = notificationManager;
            _platoUserStore = platoUserStore;
            _contextFacade = contextFacade;
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

            // For new entries we want to exclude the author from any notifications
            var usersToExclude = new List<int>()
            {
                entity.CreatedUserId
            };

            return await SendAsync(entity, usersToExclude);
        }

        async Task<TEntity> EntityUpdated(TEntity entity)
        {

            // For updated entries we want to exclude the user updating the entry from any notifications
            var usersToExclude = new List<int>()
            {
                entity.ModifiedUserId
            };

            return await SendAsync(entity, usersToExclude);

        }

        Task<TEntity> SendAsync(TEntity entity, IList<int> usersToExclude)
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

                // Send follow for specific category returning a list of notified user ids
                var notifiedUsers = await SendNotificationsForCategoryAsync(entity, usersToExclude);

                // Append excluded users to our list of already notified users
                foreach (var userToExclude in usersToExclude)
                {
                    notifiedUsers.Add(userToExclude);
                }

                // Send notifications for all categories excluding any already notified users
                await SendNotificationsForAllCategoriesAsync(entity, notifiedUsers);

            });

            return Task.FromResult(entity);

        }

        async Task<IList<int>> SendNotificationsForCategoryAsync(TEntity entity, IList<int> usersToExclude)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Compile a list of notified users
            var notifiedUsers = new List<int>();

            // The entity is NOT posted within a specific category so no need to send notifications
            if (entity.CategoryId == 0)
            {
                return notifiedUsers;
            }

            // Follow type name
            var name = FollowTypes.Category.Name;

            // Get follow state for entity
            var state = entity.GetOrCreate<FollowState>();

            // Have notifications already been sent for the entity?
            var follow = state.FollowsSent.FirstOrDefault(f =>
                f.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (follow != null)
            {
                return notifiedUsers;
            }

            // Get all follows for the category
            var follows = await _followStore.QueryAsync()
                .Select<FollowQueryParams>(q =>
                {
                    q.Name.Equals(name);
                    q.ThingId.Equals(entity.CategoryId);
                    if (usersToExclude.Count > 0)
                    {
                        q.CreatedUserId.IsNotIn(usersToExclude.ToArray());
                    }
                })
                .ToList();

            // Get all users for the follow
            var users = await ReduceUsersAsync(follows?.Data, entity);

            // No users simply return
            if (users == null)
            {
                return notifiedUsers;
            }
            

            // Send notifications
            foreach (var user in users)
            {

                // Email notifications
                if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.NewDoc))
                {

                    // Indicate user was notified
                    if (!notifiedUsers.Contains(user.Id))
                    {
                        notifiedUsers.Add(user.Id);
                    }

                    // Notify
                    await _notificationManager.SendAsync(new Notification(EmailNotifications.NewDoc)
                    {
                        To = user,
                    }, entity);

                }

                // Web notifications
                if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.NewDoc))
                {

                    // Indicate user was notified
                    if (!notifiedUsers.Contains(user.Id))
                    {
                        notifiedUsers.Add(user.Id);
                    }

                    // Notify
                    await _notificationManager.SendAsync(new Notification(WebNotifications.NewDoc)
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

            // Return a list of all notified users
            return notifiedUsers;

        }

        async Task<IList<int>> SendNotificationsForAllCategoriesAsync(TEntity entity, IList<int> usersToExclude)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Compile a list of notified users
            var notifiedUsers = new List<int>();

            // Follow type name
            var name = FollowTypes.AllCategories.Name;

            // Get follow state for entity
            var state = entity.GetOrCreate<FollowState>();

            // Have notifications already been sent for the entity?
            var follow = state.FollowsSent.FirstOrDefault(f =>
                f.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (follow != null)
            {
                return notifiedUsers;
            }

            // Get all follows
            var follows = await _followStore.QueryAsync()
                .Select<FollowQueryParams>(q =>
                {
                    q.Name.Equals(name);
                    if (usersToExclude.Count > 0)
                    {
                        q.CreatedUserId.IsNotIn(usersToExclude.ToArray());
                    }
                })
                .ToList();

            // Get all users for the follow
            var users = await ReduceUsersAsync(follows?.Data, entity);

            // No users simply return
            if (users == null)
            {
                return notifiedUsers;
            }

            // Send notifications
            foreach (var user in users)
            {

                // Email notifications
                if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.NewDoc))
                {

                    // Indicate user was notified
                    if (!notifiedUsers.Contains(user.Id))
                    {
                        notifiedUsers.Add(user.Id);
                    }

                    // Notify
                    await _notificationManager.SendAsync(new Notification(EmailNotifications.NewDoc)
                    {
                        To = user,
                    }, entity);

                }

                // Web notifications
                if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.NewDoc))
                {

                    // Indicate user was notified
                    if (!notifiedUsers.Contains(user.Id))
                    {
                        notifiedUsers.Add(user.Id);
                    }

                    // Notify
                    await _notificationManager.SendAsync(new Notification(WebNotifications.NewDoc)
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

            // Return list of notified users
            return notifiedUsers;

        }

        async Task<IEnumerable<IUser>> ReduceUsersAsync(IEnumerable<Follows.Models.Follow> follows, TEntity entity)
        {

            // We always need follows to process
            if (follows == null)
            {
                return null;
            }

            // Get all users from the follows
            var users = await _platoUserStore.QueryAsync()
                .Select<UserQueryParams>(q =>
                {
                    q.Id.IsIn(follows
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
                        entity.CategoryId, Docs.Permissions.ViewHiddenDocs))
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
                        entity.CategoryId, Docs.Permissions.ViewPrivateDocs))
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
                        entity.CategoryId, Docs.Permissions.ViewSpamDocs))
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
                        entity.CategoryId, Docs.Permissions.ViewDeletedDocs))
                    {
                        result.Remove(user.Id);
                    }
                }

            }

            return result.Count > 0 ? result.Values : null;

        }

        #endregion

    }

}
