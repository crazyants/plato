using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Users.StopForumSpam.NotificationTypes
{
    
    public class EmailNotifications : INotificationTypeProvider
    {

        public static readonly EmailNotification UserSpam =
            new EmailNotification("UserSpamEmail", "Spam Users",
                "Send me an email notification for each user detected as SPAM.");

        public IEnumerable<DefaultNotificationTypes> GetNotificationTypes()
        {
            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        UserSpam
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        UserSpam
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
                        UserSpam
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        UserSpam
                    }
                }

            };
        }

    }

}
