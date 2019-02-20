using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.StopForumSpam.NotificationTypes
{

    public class WebNotifications : INotificationTypeProvider
    {

        public static readonly WebNotification TopicSpam =
            new WebNotification("TopicSpamWeb", "Spam Topics",
                "Show me a web notification for each topic detected as SPAM.");

        public static readonly WebNotification ReplySpam =
            new WebNotification("ReplySpamWeb", "Spam Replies",
                "Show me a web notification for each reply detected as SPAM.");
        
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
