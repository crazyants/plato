using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Notifications.Abstractions.Models;

namespace Plato.Mentions.Notifications
{
    public class WebNotifications : INotificationTypeProvider
    {

        public static readonly WebNotification NewMention =
            new WebNotification("NewMentionWeb", "New Mentions", "Send me a email notification for each @mention.", "Email");

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
