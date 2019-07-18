using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Plato.Discuss.Models;
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
using Plato.Internal.Security.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Stores;

namespace Plato.Discuss.Tags.Follow.Subscribers
{
    public class EntityReplySubscriber<TEntityReply> : IBrokerSubscriber where TEntityReply : class, IEntityReply
    {

        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;
        private readonly IDummyClaimsPrincipalFactory<User> _claimsPrincipalFactory;
        private readonly INotificationManager<TEntityReply> _notificationManager;
        private readonly IEntityReplyStore<TEntityReply> _entityReplyStore;
        private readonly IFollowStore<Follows.Models.Follow> _followStore;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityTagStore<EntityTag> _entityTagStore;
        private readonly IDeferredTaskManager _deferredTaskManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IBroker _broker;

        public EntityReplySubscriber(
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            IDummyClaimsPrincipalFactory<User> claimsPrincipalFactory,
            INotificationManager<TEntityReply> notificationManager,
            IEntityReplyStore<TEntityReply> entityReplyStore,
            IFollowStore<Follows.Models.Follow> followStore,
            IEntityTagStore<EntityTag> entityTagStore,
            IAuthorizationService authorizationService,
            IDeferredTaskManager deferredTaskManager,
            IPlatoUserStore<User> platoUserStore,
            IEntityStore<Topic> entityStore,
            IBroker broker)
        {
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _authorizationService = authorizationService;
            _deferredTaskManager = deferredTaskManager;
            _notificationManager = notificationManager;
            _entityReplyStore = entityReplyStore;
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
            _broker.Sub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyCreated",
                Order = short.MaxValue
            }, async message => await EntityReplyCreated(message.What));

            // Updated
            _broker.Sub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyUpdated",
                Order = short.MaxValue
            }, async message => await EntityReplyUpdated(message.What));

        }

        public void Unsubscribe()
        {
            // Created
            _broker.Unsub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyCreated",
                Order = short.MaxValue
            }, async message => await EntityReplyCreated(message.What));

            // Updated
            _broker.Unsub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyUpdated",
                Order = short.MaxValue
            }, async message => await EntityReplyUpdated(message.What));

        }

        #endregion

        #region "Private Methods"

        async Task<TEntityReply> EntityReplyCreated(TEntityReply reply)
        {

            // For new replies we want to exclude the author from any notifications
            var usersToExclude = new List<int>()
            {
                reply.CreatedUserId
            };

            return await SendAsync(reply, usersToExclude);
        }

        async Task<TEntityReply> EntityReplyUpdated(TEntityReply reply)
        {

            // For updated replies we want to exclude the user updating the entry from any notifications
            var usersToExclude = new List<int>()
            {
                reply.ModifiedUserId
            };

            return await SendAsync(reply, usersToExclude);

        }

        async Task<TEntityReply> SendAsync(TEntityReply reply, IList<int> usersToExclude)
        {

            if (reply == null)
            {
                throw new ArgumentNullException(nameof(reply));
            }

            // Get the entity for the reply
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return reply;
            }

            // We don't need to trigger notifications for hidden entities
            if (entity.IsHidden())
            {
                return reply;
            }

            // Defer notifications 
            _deferredTaskManager.AddTask(async context =>
            {
                await SendNotificationsAsync(reply, usersToExclude);
            });

            return reply;

        }

        async Task<IList<int>> SendNotificationsAsync(TEntityReply reply, IList<int> usersToExclude)
        {

            if (reply == null)
            {
                throw new ArgumentNullException(nameof(reply));
            }

            // Compile a list of notified users
            var notifiedUsers = new List<int>();

            // Follow type name
            var name = FollowTypes.Tag.Name;

            // Get follow state for reply
            var state = reply.GetOrCreate<FollowState>();

            // Have notifications already been sent for the reply?
            var follow = state.FollowsSent.FirstOrDefault(f =>
                f.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (follow != null)
            {
                return notifiedUsers;
            }

            // Compile all entity tags ids for the entity
            var tags = new List<int>();
            var entityTags = await _entityTagStore.GetByEntityReplyIdAsync(reply.Id);
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

            // Get follows for all reply tags
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
            var users = await ReduceUsersAsync(follows?.Data, reply);

            // No users simply return
            if (users == null)
            {
                return notifiedUsers;
            }

            // Send notifications
            foreach (var user in users)
            {

                // Email notifications
                if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.NewReplyTag))
                {

                    // Track notified
                    if (!notifiedUsers.Contains(user.Id))
                    {
                        notifiedUsers.Add(user.Id);
                    }

                    // Notify
                    await _notificationManager.SendAsync(new Notification(EmailNotifications.NewReplyTag)
                    {
                        To = user,
                    }, reply);
                }

                // Web notifications
                if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.NewReplyTag))
                {

                    // Track notified
                    if (!notifiedUsers.Contains(user.Id))
                    {
                        notifiedUsers.Add(user.Id);
                    }

                    // Notify
                    await _notificationManager.SendAsync(new Notification(WebNotifications.NewReplyTag)
                    {
                        To = user,
                        From = new User
                        {
                            Id = reply.CreatedBy.Id,
                            UserName = reply.CreatedBy.UserName,
                            DisplayName = reply.CreatedBy.DisplayName,
                            Alias = reply.CreatedBy.Alias,
                            PhotoUrl = reply.CreatedBy.PhotoUrl,
                            PhotoColor = reply.CreatedBy.PhotoColor
                        }
                    }, reply);

                }

            }

            // Update state
            state.AddSent(name);
            reply.AddOrUpdate(state);

            // Persist state
            await _entityReplyStore.UpdateAsync(reply);

            // Return a list of all notified users
            return notifiedUsers;

        }

        async Task<IEnumerable<IUser>> ReduceUsersAsync(IEnumerable<Follows.Models.Follow> follows, TEntityReply reply)
        {

            // We always need follows to process
            if (follows == null)
            {
                return null;
            }
            
            // Get the entity for the reply
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);

            // We always need an entity
            if (entity == null)
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

                // If the reply is hidden but the user does
                // not have permission to view hidden replies
                if (reply.IsHidden)
                {
                    var principal = await _claimsPrincipalFactory.CreateAsync(user);
                    if (!await _authorizationService.AuthorizeAsync(principal,
                        entity.CategoryId, Discuss.Permissions.ViewHiddenReplies))
                    {
                        result.Remove(user.Id);
                    }
                }
                
                // The reply has been flagged as SPAM but the user does
                // not have permission to view replies flagged as SPAM
                if (reply.IsSpam)
                {
                    var principal = await _claimsPrincipalFactory.CreateAsync(user);
                    if (!await _authorizationService.AuthorizeAsync(principal,
                        entity.CategoryId, Discuss.Permissions.ViewSpamReplies))
                    {
                        result.Remove(user.Id);
                    }
                }

                // The reply is soft deleted but the user does 
                // not have permission to view soft deleted replies
                if (reply.IsDeleted)
                {
                    var principal = await _claimsPrincipalFactory.CreateAsync(user);
                    if (!await _authorizationService.AuthorizeAsync(principal,
                        entity.CategoryId, Discuss.Permissions.ViewDeletedReplies))
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
