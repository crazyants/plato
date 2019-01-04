using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Discuss.Channels.Follow.NotificationTypes
{

    public class EmailNotifications : INotificationTypeProvider
    {
        
        public static readonly EmailNotification NewTopic =
            new EmailNotification("NewTopicEmail", "New Topic", "Send me an email notification for each new topic within channels i'm following.");

        public IEnumerable<INotificationType> GetNotificationTypes()
        {
            return new[]
            {
                NewTopic,
            };
        }

        public IEnumerable<INotificationType> GetDefaultNotificationTypes()
        {
            return new[]
            {
                NewTopic,
            };
        }
    }

}
