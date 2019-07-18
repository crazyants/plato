using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.Labels.Follow.NotificationTypes
{

    public class EmailNotifications : INotificationTypeProvider
    {
        
        public static readonly EmailNotification NewLabel =
            new EmailNotification(
                "NewTopicLabelEmail",
                "Topic Labels",
                "Send me an email notification for each new topic associated with a label I'm following.");

        public IEnumerable<DefaultNotificationTypes> GetNotificationTypes()
        {
            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        NewLabel
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        NewLabel
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Member,
                    NotificationTypes = new[]
                    {
                        NewLabel
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
                        NewLabel
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        NewLabel
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Member,
                    NotificationTypes = new[]
                    {
                        NewLabel
                    }
                }

            };

        }
        
    }

}
