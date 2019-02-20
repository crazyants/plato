using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.StopForumSpam.NotificationTypes
{
    
    public class EmailNotifications : INotificationTypeProvider
    {

        public static readonly EmailNotification TopicSpam =
            new EmailNotification("TopicSpamEmail", "Spam Topics",
                "Send me an email notification for each topic detected as SPAM.");

        public static readonly EmailNotification ReplySpam =
            new EmailNotification("ReplySpamEmail", "Spam Replies",
                "Send me an email notification for each reply detected as SPAM.");


        public IEnumerable<DefaultNotificationTypes> GetNotificationTypes()
        {
            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        TopicSpam,
                        ReplySpam
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        TopicSpam,
                        ReplySpam
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
                        TopicSpam,
                        ReplySpam
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        TopicSpam,
                        ReplySpam
                    }
                }

            };
        }

    }

}
