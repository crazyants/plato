using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Mentions.Notifications
{

    public class WebNotifications : INotificationTypeProvider
    {

        public static readonly WebNotification NewMention =
            new WebNotification("NewMentionWeb", "New Mentions", "Send me a email notification for each @mention.");

        public IEnumerable<INotificationType> GetNotificationTypes()
        {
            return new[]
            {
                NewMention
            };
        }

        public IEnumerable<INotificationType> GetDefaultPermissions()
        {
            return null;
        }

    }

}
