using System.Threading.Tasks;
using Plato.Ideas.Models;
using Plato.Ideas.NotificationTypes;
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

namespace Plato.Ideas.Services
{

    public class ReportIdeaCommentManager : IReportEntityManager<IdeaComment>
    {

        private readonly INotificationManager<ReportSubmission<IdeaComment>> _notificationManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;
        private readonly IDeferredTaskManager _deferredTaskManager;

        public ReportIdeaCommentManager(
            INotificationManager<ReportSubmission<IdeaComment>> notificationManager,
            IPlatoUserStore<User> platoUserStore,
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            IDeferredTaskManager deferredTaskManager)
        {
            _notificationManager = notificationManager;
            _platoUserStore = platoUserStore;
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
            _deferredTaskManager = deferredTaskManager;
        }
        
        public Task ReportAsync(ReportSubmission<IdeaComment> submission)
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
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.IdeaCommentReport))
                    {
                        await _notificationManager.SendAsync(new Notification(WebNotifications.IdeaCommentReport)
                        {
                            To = user,
                            From = from
                        }, submission);
                    }

                    // Email notification
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.IdeaCommentReport))
                    {
                        await _notificationManager.SendAsync(new Notification(EmailNotifications.IdeaCommentReport)
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

