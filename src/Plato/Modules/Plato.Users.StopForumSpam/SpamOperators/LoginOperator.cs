using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;
using Plato.Internal.Tasks.Abstractions;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.Users.StopForumSpam.NotificationTypes;

namespace Plato.Users.StopForumSpam.SpamOperators
{

    public class LoginOperator : ISpamOperatorProvider<User>
    {

        private readonly ISpamChecker _spamChecker;
        private readonly INotificationManager<User> _notificationManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IDeferredTaskManager _deferredTaskManager;

        public LoginOperator(ISpamChecker spamChecker,
            INotificationManager<User> notificationManager,
            IPlatoUserStore<User> platoUserStore, 
            IDeferredTaskManager deferredTaskManager)
        {
            _spamChecker = spamChecker;
            _notificationManager = notificationManager;
            _platoUserStore = platoUserStore;
            _deferredTaskManager = deferredTaskManager;
        }

        public async Task<ISpamOperatorResult<User>> ValidateModelAsync(ISpamOperatorContext<User> context)
        {

            // Ensure correct operation provider
            if (!context.Operation.Name.Equals(SpamOperations.Login.Name, StringComparison.Ordinal))
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
                    user.IsSpam = true;
                    user.IsSpamUpdatedUserId = bot?.Id ?? 0;
                    user.IsSpamUpdatedDate = DateTimeOffset.UtcNow;
                    await _platoUserStore.UpdateAsync(user);
                }
            }

            // Defer notifications
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
                await _notificationManager.SendAsync(new Notification(WebNotifications.UserSpam)
                {
                    To = user,
                    From = bot
                }, context.Model);

                // Email notification
                await _notificationManager.SendAsync(new Notification(EmailNotifications.UserSpam)
                {
                    To = user
                }, context.Model);

            }
        
        }

        async Task<IEnumerable<User>> GetUsersAsync(ISpamOperation operation)
        {

            List<User> output = null;

            // Notify administrators 
            if (operation.NotifyAdmin)
            {
                var users = await _platoUserStore.QueryAsync()
                    .Select<UserQueryParams>(q =>
                    {
                        q.RoleName.Equals(DefaultRoles.Administrator);
                    })
                    .ToList();
                if (users?.Data != null)
                {
                    output = new List<User>();
                    output.AddRange(users.Data);
                }
            }

            // Notify staff 
            if (operation.NotifyStaff)
            {
                var users = await _platoUserStore.QueryAsync()
                    .Select<UserQueryParams>(q =>
                    {
                        q.RoleName.Equals(DefaultRoles.Staff);
                    })
                    .ToList();
                if (users?.Data != null)
                {
                    if (output == null)
                    {
                        output = new List<User>();
                    }
                    output.AddRange(users.Data);
                }
            }

            return output?.Distinct().ToList();

        }
        
    }

}


