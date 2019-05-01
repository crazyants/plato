using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Issues.NotificationTypes
{

    public class WebNotifications : INotificationTypeProvider
    {

        public static readonly WebNotification IssueReport =
            new WebNotification("IssueReportWeb", "Reported Issues",
                "Show me a web notification each time an issue is reported.");

        public static readonly WebNotification IssueCommentReport =
            new WebNotification("IssueCommentReportWeb", "Reported Issue Comments",
                "Show me a web notification each time an issue comment is reported.");
        
        public IEnumerable<DefaultNotificationTypes> GetNotificationTypes()
        {
            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        IssueReport,
                        IssueCommentReport
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        IssueReport,
                        IssueCommentReport
                    }
                }

            };
        }

        public IEnumerable<DefaultNotificationTypes> GetDefaultNotificationTypes()
        {
            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        IssueReport,
                        IssueCommentReport
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        IssueReport,
                        IssueCommentReport
                    }
                }
            };
        }

    }

}
