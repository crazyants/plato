using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Discuss.Mentions.NotificationTypes
{

    public class WebNotifications : INotificationTypeProvider
    {

        public static readonly WebNotification NewMention =
            new WebNotification("NewMentionWeb", "New Mentions",
                "Show me a web notification for each new @mention.");

        public IEnumerable<INotificationType> GetNotificationTypes()
        {
            return new[]
            {
                NewMention
            };
        }

        public IEnumerable<INotificationType> GetDefaultNotificationTypes()
        {
            return new[]
            {
                NewMention
            };
        }

    }

}
