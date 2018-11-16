using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Discuss.NotificationTypes
{

    public class WebNotifications : INotificationTypeProvider
    {

        public static readonly WebNotification NewTopics =
            new WebNotification("NewTopicWeb", "New Topics", "Show me a web notification for each new topic.");

        public static readonly WebNotification NewReplies =
            new WebNotification("NewReplyWeb", "New Replies", "Show me a web notification for each new reply.");

        public IEnumerable<INotificationType> GetNotificationTypes()
        {
            return new[]
            {
                NewTopics,
                NewReplies,
            };
        }

        public IEnumerable<INotificationType> GetDefaultPermissions()
        {
            return new[]
            {
                NewTopics,
                NewReplies,
            };
        }

    }

}
