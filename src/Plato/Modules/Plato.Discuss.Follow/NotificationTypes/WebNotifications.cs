using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Discuss.Follow.NotificationTypes
{

    public class WebNotifications : INotificationTypeProvider
    {

        public static readonly WebNotification NewTopics =
            new WebNotification("NewTopicWeb", "New Topics", "Show me a web notification for each new topic.");

        public static readonly WebNotification NewReply =
            new WebNotification("NewReplyWeb", "New Replies", "Show me a web notification for replies within topics i'm following.");

        public IEnumerable<INotificationType> GetNotificationTypes()
        {
            return new[]
            {
                NewTopics,
                NewReply,
            };
        }

        public IEnumerable<INotificationType> GetDefaultNotificationTypes()
        {
            return new[]
            {
                NewTopics,
                NewReply,
            };
        }

    }

}
