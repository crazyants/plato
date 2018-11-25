using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Discuss.NotificationTypes
{

    public class EmailNotifications : INotificationTypeProvider
    {

        public static readonly EmailNotification NewTopics =
            new EmailNotification("NewTopicsEmail", "New Topics", "Send me a email notification for each new topic.");

        public static readonly EmailNotification NewReplies =
            new EmailNotification("NewRepliesEmail", "New Replies", "Send me an email notification for each new reply.");

        public IEnumerable<INotificationType> GetNotificationTypes()
        {
            return new[]
            {
                NewTopics,
                NewReplies,
            };
        }

        public IEnumerable<INotificationType> GetDefaultNotificationTypes()
        {
            return new[]
            {
                NewTopics
            };
        }
    }

}
