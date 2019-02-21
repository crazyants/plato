using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Notifications.Extensions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;
using Plato.Internal.Tasks.Abstractions;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.Users.StopForumSpam.NotificationTypes;

namespace Plato.Users.StopForumSpam.SpamOperators
{
    public class RegistrationOperator : Plato.StopForumSpam.Services.ISpamOperatorProvider<User>
    {
        
        private readonly ISpamChecker _spamChecker;
        private readonly INotificationManager<User> _notificationManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IDeferredTaskManager _deferredTaskManager;
        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;
        
        public RegistrationOperator(
            ISpamChecker spamChecker,
            INotificationManager<User> notificationManager,
            IPlatoUserStore<User> platoUserStore,
            IDeferredTaskManager deferredTaskManager,
            IUserNotificationTypeDefaults userNotificationTypeDefaults)
        {
            _spamChecker = spamChecker;
            _notificationManager = notificationManager;
            _platoUserStore = platoUserStore;
            _deferredTaskManager = deferredTaskManager;
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
        }

        public async Task<ISpamOperatorResult<User>> ValidateModelAsync(ISpamOperatorContext<User> context)
        {

            // Ensure correct operation provider
            if (!context.Operation.Name.Equals(SpamOperations.Registration.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Create result
            var result = new SpamOperatorResult<User>();

            // User is OK
            var spamResult = await _spamChecker.CheckAsync(context.Model);
            if (spamResult.Succeeded)
            {
                return result.Success(context.Model);
            }

            // Return failed with our updated model and operation
            // This provides the calling code with the operation error message
            return result.Failed(context.Model, context.Operation);

        }

        public async Task<ISpamOperatorResult<User>> UpdateModelAsync(ISpamOperatorContext<User> context)
        {

            // Perform validation
            var validation = await ValidateModelAsync(context);

            // Create result
            var result = new SpamOperatorResult<User>();

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

            // Flag user as SPAM?
            if (context.Operation.FlagAsSpam)
            {
                var bot = await _platoUserStore.GetPlatoBotAsync();
                var user = await _platoUserStore.GetByUserNameAsync(context.Model.UserName);
                if (user != null)
                {
                    if (!user.IsSpam)
                    {
                        user.IsSpam = true;
                        user.IsSpamUpdatedUserId = bot?.Id ?? 0;
                        user.IsSpamUpdatedDate = DateTimeOffset.UtcNow;
                        await _platoUserStore.UpdateAsync(user);
                    }
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

        async Task NotifyAsync(ISpamOperatorContext<User> context)
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
                if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.UserSpam))
                {
                    await _notificationManager.SendAsync(new Notification(WebNotifications.UserSpam)
                    {
                        To = user,
                        From = bot
                    }, context.Model);
                }

                // Email notification
                if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.UserSpam))
                {
                    await _notificationManager.SendAsync(new Notification(EmailNotifications.UserSpam)
                    {
                        To = user
                    }, context.Model);
                }

            }

        }

        async Task<IEnumerable<User>> GetUsersAsync(ISpamOperation operation)
        {

            var roleNames = new List<string>();
            if (operation.NotifyAdmin)
                roleNames.Add(DefaultRoles.Administrator);
            if (operation.NotifyStaff)
                roleNames.Add(DefaultRoles.Staff);
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
