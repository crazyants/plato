using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Plato.Discuss.Follow.NotificationTypes;
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

namespace Plato.Discuss.Follow.Subscribers
{
  
    /// <summary>
    /// Triggers all entity follow notifications when public replies are posted.
    /// </summary>
    /// <typeparam name="TEntityReply"></typeparam>
    public class EntityReplySubscriber<TEntityReply> : IBrokerSubscriber where TEntityReply : class, IEntityReply
    {
        
        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;
        private readonly IDummyClaimsPrincipalFactory<User> _claimsPrincipalFactory;
        private readonly INotificationManager<TEntityReply> _notificationManager;
        private readonly IFollowStore<Follows.Models.Follow> _followStore;
        private readonly IAuthorizationService _authorizationService;
        private readonly IDeferredTaskManager _deferredTaskManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IEntityStore<Entity> _entityStore;
        private readonly IBroker _broker;
        
        public EntityReplySubscriber(
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            IDummyClaimsPrincipalFactory<User> claimsPrincipalFactory,
            INotificationManager<TEntityReply> notificationManager,
            IFollowStore<Follows.Models.Follow> followStore,
            IAuthorizationService authorizationService,
            IDeferredTaskManager deferredTaskManager,
            IPlatoUserStore<User> platoUserStore,
            IEntityStore<Entity> entityStore,
            IBroker broker)
        {
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _authorizationService = authorizationService;
            _notificationManager = notificationManager;
            _deferredTaskManager = deferredTaskManager;
            _platoUserStore = platoUserStore;
            _entityStore = entityStore;
            _followStore = followStore;
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

        Task<TEntityReply> EntityReplyCreated(TEntityReply reply)
        {

       
            if (reply == null)
            {
                throw new ArgumentNullException(nameof(reply));
            }

            // The reply always need an entity Id
            if (reply.EntityId <= 0)
            {
                throw new ArgumentNullException(nameof(reply.EntityId));
            }

            // No need to send notifications for hidden replies
            if (reply.IsHidden())
            {
                return Task.FromResult(reply);
            }
            
            // Add deferred task
            _deferredTaskManager.AddTask(async context =>
            {

                // Follow type name
                var name = FollowTypes.Topic.Name;

                // Get follow state for reply
                //var state = reply.GetOrCreate<FollowState>();

                //// Have notifications already been sent for the entity?
                //var follow = state.FollowsSent.FirstOrDefault(f =>
                //    f.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                //if (follow != null)
                //{
                //    return;
                //}
                
                // Get entity for reply
                var entity = await _entityStore.GetByIdAsync(reply.EntityId);

                // No need to send notifications if the entity is hidden
                if (entity.IsHidden())
                {
                    return;
                }
                
                // Get all follows for topic
                var follows = await _followStore.QueryAsync()
                    .Select<FollowQueryParams>(q =>
                    {
                        q.ThingId.Equals(reply.EntityId);
                        q.Name.Equals(name);
                    })
                    .ToList();

                // No follows simply return
                if (follows?.Data == null)
                {
                    return;
                }

                var users = await GetUsersAsync(follows?.Data, reply);

                if (users == null)
                {
                    return;
                }
                
                // Send notifications
                foreach (var user in users)
                {

                    // Email notifications
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.NewReply))
                    {
                        await _notificationManager.SendAsync(new Notification(EmailNotifications.NewReply)
                        {
                            To = user,
                        }, reply);
                    }

                    // Web notifications
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.NewReply))
                    {
                        await _notificationManager.SendAsync(new Notification(WebNotifications.NewReply)
                        {
                            To = user,
                            From = new User()
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


                //// Update state
                //state.AddSent(name);
                //entity.AddOrUpdate(state);

                //// Persist state
                //await _entityStore.UpdateAsync(entity);

            });

           
            return Task.FromResult(reply);
            
        }

        Task<TEntityReply> EntityReplyUpdated(TEntityReply reply)
        {
            // No need to send notifications for reply updates
            // May be implemented at a later stage
            return Task.FromResult(reply);
        }


        // TODO: Look at centralizing within class
        async Task<IEnumerable<IUser>> GetUsersAsync(
            IEnumerable<Follows.Models.Follow> follows,
            TEntityReply reply)
        {

            // We always need follows to process
            if (follows == null)
            {
                return null;
            }
            
            // Get entity for reply
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);

            // No need to send notifications if the entity is hidden
            if (entity.IsHidden())
            {
                return null;
            }
            

            // Get all users following the category
            // Exclude the author so they are not notified of there own posts
            var users = await _platoUserStore.QueryAsync()
                .Select<UserQueryParams>(q =>
                {
                    q.Id.IsIn(follows
                        .Where(f => f.CreatedUserId != reply.CreatedUserId)
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
                if (reply.IsHidden)
                {
                    var principal = await _claimsPrincipalFactory.CreateAsync(user);
                    if (!await _authorizationService.AuthorizeAsync(principal,
                        entity.CategoryId, Discuss.Permissions.ViewHiddenReplies))
                    {
                        result.Remove(user.Id);
                    }
                }

                // The entity has been flagged as SPAM but the user does
                // not have permission to view entities flagged as SPAM
                if (reply.IsSpam)
                {
                    var principal = await _claimsPrincipalFactory.CreateAsync(user);
                    if (!await _authorizationService.AuthorizeAsync(principal,
                        entity.CategoryId, Discuss.Permissions.ViewSpamReplies))
                    {
                        result.Remove(user.Id);
                    }
                }

                // The entity is soft deleted but the user does 
                // not have permission to view soft deleted entities
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

            return result.Values;

        }


        #endregion

    }

}
