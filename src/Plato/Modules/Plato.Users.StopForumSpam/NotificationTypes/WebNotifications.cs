using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Users.StopForumSpam.NotificationTypes
{

    public class WebNotifications : INotificationTypeProvider
    {

        public static readonly WebNotification UserSpam =
            new WebNotification("UserSpamWeb", "New User Spam",
                "Show me a web notification for each new user detected as SPAM.");

        public IEnumerable<INotificationType> GetNotificationTypes()
        {
            return new[]
            {
                UserSpam
            };
        }

        public IEnumerable<INotificationType> GetDefaultNotificationTypes()
        {
            return new[]
            {
                UserSpam
            };
        }

    }

}
