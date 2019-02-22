using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Notifications.Extensions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;
using Plato.Internal.Tasks.Abstractions;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.Discuss.Models;
using Plato.Discuss.StopForumSpam.NotificationTypes;
using Plato.Entities.Stores;

namespace Plato.Discuss.StopForumSpam.SpamOperators
{

    public class TopicOperator : ISpamOperatorProvider<Topic>
    {

        private readonly ISpamChecker _spamChecker;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IEntityStore<Topic> _topicStore;
        private readonly IDeferredTaskManager _deferredTaskManager;
        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;
        private readonly INotificationManager<Topic> _notificationManager;

        public TopicOperator(
            ISpamChecker spamChecker,
            IPlatoUserStore<User> platoUserStore,
            IEntityStore<Topic> topicStore,
            IDeferredTaskManager deferredTaskManager,
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            INotificationManager<Topic> notificationManager)
        {
            _spamChecker = spamChecker;
            _platoUserStore = platoUserStore;
            _topicStore = topicStore;
            _deferredTaskManager = deferredTaskManager;
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
            _notificationManager = notificationManager;
        }

        public async Task<ISpamOperatorResult<Topic>> ValidateModelAsync(ISpamOperatorContext<Topic> context)
        {

            // Ensure correct operation provider
            if (!context.Operation.Name.Equals(SpamOperations.Topic.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Get user for topic
            var user = await _platoUserStore.GetByIdAsync(context.Model.CreatedUserId);
            if (user == null)
            {
                return null;
            }

            // Create result
            var result = new SpamOperatorResult<Topic>();

            // User is OK
            var spamResult = await _spamChecker.CheckAsync(user);
            if (spamResult.Succeeded)
            {
                return result.Success(context.Model);
            }

            // Return failed with our updated model and operation
            // This provides the calling code with the operation error message
            return result.Failed(context.Model, context.Operation);


        }

        public async Task<ISpamOperatorResult<Topic>> UpdateModelAsync(ISpamOperatorContext<Topic> context)
        {

            // Perform validation
            var validation = await ValidateModelAsync(context);

            // Create result
            var result = new SpamOperatorResult<Topic>();
            
            // Not an operator of interest
            if (validation == null)
            {
                return result.Success(context.Model);
            }

            // If validation succeeded no need to perform further actions
            if (validation.Succeeded)
            {
                return result.Success(context.Model);
            }
            
            // Get topic author
            var user = await _platoUserStore.GetByIdAsync(context.Model.CreatedUserId);
            if (user == null)
            {
                return null;
            }
            
            // Flag user as SPAM?
            if (context.Operation.FlagAsSpam)
            {
                var bot = await _platoUserStore.GetPlatoBotAsync();

                // Mark user as SPAM
                if (!user.IsSpam)
                {
                    user.IsSpam = true;
                    user.IsSpamUpdatedUserId = bot?.Id ?? 0;
                    user.IsSpamUpdatedDate = DateTimeOffset.UtcNow;
                    await _platoUserStore.UpdateAsync(user);
                }

                // Mark topic as SPAM
                if (!context.Model.IsSpam)
                {
                    context.Model.IsSpam = true;
                    await _topicStore.UpdateAsync(context.Model);
                }

            }

            // Defer notifications for execution after request completes
            _deferredTaskManager.AddTask(async ctx =>
            {
                await NotifyAsync(context);
            });

            // Return failed with our updated model and operation
            // This provides the calling code with the operation error message
            return result.Failed(context.Model, context.Operation);

        }

        async Task NotifyAsync(ISpamOperatorContext<Topic> context)
        {

            // Get users to notify
            var users = await GetUsersAsync(context.Operation);

            // No users to notify
            if (users == null)
            {
                return;
            }

            // Get bot
            var bot = await _platoUserStore.GetPlatoBotAsync();

            // Send notifications
            foreach (var user in users)
            {

                // Web notification
                if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.TopicSpam))
                {
                    await _notificationManager.SendAsync(new Notification(WebNotifications.TopicSpam)
                    {
                        To = user,
                        From = bot
                    }, context.Model);
                }

                // Email notification
                if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.TopicSpam))
                {
                    await _notificationManager.SendAsync(new Notification(EmailNotifications.TopicSpam)
                    {
                        To = user
                    }, context.Model);
                }

            }

        }

        async Task<IEnumerable<User>> GetUsersAsync(ISpamOperation operation)
        {

            var roleNames = new List<string>(2);
            if (operation.NotifyAdmin)
                roleNames.Add(DefaultRoles.Administrator);
            if (operation.NotifyStaff)
                roleNames.Add(DefaultRoles.Staff);
            if (roleNames.Count == 0)
                return null;
            var users = await _platoUserStore.QueryAsync()
                .Select<UserQueryParams>(q =>
                {
                    q.RoleName.IsIn(roleNames.ToArray());
                })
                .ToList();
            return users?.Data;

        }

    }
    
}


