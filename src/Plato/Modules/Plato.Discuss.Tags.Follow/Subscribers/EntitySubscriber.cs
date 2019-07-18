using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Plato.Discuss.Tags.Follow.NotificationTypes;
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
using Plato.Tags.Models;
using Plato.Tags.Stores;

namespace Plato.Discuss.Tags.Follow.Subscribers
{
    public class EntitySubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {

        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;
        private readonly IDummyClaimsPrincipalFactory<User> _claimsPrincipalFactory;
        private readonly INotificationManager<TEntity> _notificationManager;
        private readonly IFollowStore<Follows.Models.Follow> _followStore;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityTagStore<EntityTag> _entityTagStore;
        private readonly IDeferredTaskManager _deferredTaskManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IEntityStore<TEntity> _entityStore;
        private readonly IBroker _broker;

        public EntitySubscriber(
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            IDummyClaimsPrincipalFactory<User> claimsPrincipalFactory,
            INotificationManager<TEntity> notificationManager,
            IFollowStore<Follows.Models.Follow> followStore,
            IEntityTagStore<EntityTag> entityTagStore,
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
            _entityTagStore = entityTagStore;
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

            // Defer notifications 
            _deferredTaskManager.AddTask(async context =>
            {
                await SendNotificationsAsync(entity, usersToExclude);
            });

            return Task.FromResult(entity);

        }

        async Task<IList<int>> SendNotificationsAsync(TEntity entity, IList<int> usersToExclude)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Compile a list of notified users
            var notifiedUsers = new List<int>();

            // Follow type name
            var name = FollowTypes.Tag.Name;

            // Get follow state for entity
            var state = entity.GetOrCreate<FollowState>();

            // Have notifications already been sent for the entity?
            var follow = state.FollowsSent.FirstOrDefault(f =>
                f.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (follow != null)
            {
                return notifiedUsers;
            }

            // Compile all entity tags ids for the entity
            var tags = new List<int>();
            var entityTags = await _entityTagStore.GetByEntityIdAsync(entity.Id);
            if (entityTags != null)
            {
                foreach (var entityTag in entityTags)
                {
                    if (!tags.Contains(entityTag.TagId))
                    {
                        tags.Add(entityTag.TagId);
                    }
                }
            }

            // Get follows for all entity tags
            var follows = await _followStore.QueryAsync()
                .Select<FollowQueryParams>(q =>
                {
                    q.Name.Equals(name);
                    q.ThingId.IsIn(tags.ToArray());
                    if (usersToExclude.Count > 0)
                    {
                        q.CreatedUserId.IsNotIn(usersToExclude.ToArray());
                    }
                })
                .ToList();

            // Reduce the users for all found follows
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
                if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.NewTag))
                {

                    // Track notified
                    if (!notifiedUsers.Contains(user.Id))
                    {
                        notifiedUsers.Add(user.Id);
                    }

                    // Notify
                    await _notificationManager.SendAsync(new Notification(EmailNotifications.NewTag)
                    {
                        To = user,
                    }, entity);
                }

                // Web notifications
                if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.NewTag))
                {

                    // Track notified
                    if (!notifiedUsers.Contains(user.Id))
                    {
                        notifiedUsers.Add(user.Id);
                    }

                    // Notify
                    await _notificationManager.SendAsync(new Notification(WebNotifications.NewTag)
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

        async Task<IEnumerable<IUser>> ReduceUsersAsync(IEnumerable<Follows.Models.Follow> follows, TEntity entity)
        {

            // We always need follows to process
            if (follows == null)
            {
                return null;
            }

            // Get all users from the supplied follows
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

                // Ensure the user is only added once
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
                        entity.CategoryId, Discuss.Permissions.ViewHiddenTopics))
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
                        entity.CategoryId, Discuss.Permissions.ViewPrivateTopics))
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
                        entity.CategoryId, Discuss.Permissions.ViewSpamTopics))
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
                        entity.CategoryId, Discuss.Permissions.ViewDeletedTopics))
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
