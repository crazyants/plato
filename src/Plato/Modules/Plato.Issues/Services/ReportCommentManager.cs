using System.Threading.Tasks;
using Plato.Issues.Models;
using Plato.Issues.NotificationTypes;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Notifications.Extensions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Issues.Services
{

    public class ReportCommentManager : IReportEntityManager<Comment>
    {

        private readonly INotificationManager<ReportSubmission<Comment>> _notificationManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;
        private readonly IDeferredTaskManager _deferredTaskManager;

        public ReportCommentManager(
            INotificationManager<ReportSubmission<Comment>> notificationManager,
            IPlatoUserStore<User> platoUserStore,
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            IDeferredTaskManager deferredTaskManager)
        {
            _notificationManager = notificationManager;
            _platoUserStore = platoUserStore;
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
            _deferredTaskManager = deferredTaskManager;
        }
        
        public Task ReportAsync(ReportSubmission<Comment> submission)
        {

            // Defer notifications for execution after request completes
            _deferredTaskManager.AddTask(async ctx =>
            {

                // Get users to notify
                var users = await _platoUserStore.QueryAsync()
                    .Select<UserQueryParams>(q =>
                    {
                        q.RoleName.IsIn(new[]
                        {
                            DefaultRoles.Administrator,
                            DefaultRoles.Staff
                        });
                    })
                    .ToList();

                // No users to notify
                if (users?.Data == null)
                {
                    return;
                }

                var from = submission.Who ?? await _platoUserStore.GetPlatoBotAsync();

                // Send notifications
                foreach (var user in users.Data)
                {

                    // Web notification
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.IssueCommentReport))
                    {
                        await _notificationManager.SendAsync(new Notification(WebNotifications.IssueCommentReport)
                        {
                            To = user,
                            From = from
                        }, submission);
                    }

                    // Email notification
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.IssueCommentReport))
                    {
                        await _notificationManager.SendAsync(new Notification(EmailNotifications.IssueCommentReport)
                        {
                            To = user
                        }, submission);
                    }

                }

            });

            return Task.CompletedTask;

        }

    }

}
