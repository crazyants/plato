using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Discuss.Channels.Follow.NotificationTypes
{

    public class WebNotifications : INotificationTypeProvider
    {

        public static readonly WebNotification NewTopic =
            new WebNotification("NewTopicWeb", "New Topics", "Show me a web notification for each new topic within channels i'm following.");

        public IEnumerable<INotificationType> GetNotificationTypes()
        {
            return new[]
            {
                NewTopic
            };
        }

        public IEnumerable<INotificationType> GetDefaultNotificationTypes()
        {
            return new[]
            {
                NewTopic
            };
        }

    }

}
