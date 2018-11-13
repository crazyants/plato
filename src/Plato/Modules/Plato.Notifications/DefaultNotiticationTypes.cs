using System;
using System.Collections.Generic;
using Plato.Notifications.Models;
using Plato.Notifications.Services;

namespace Plato.Notifications
{
    public class DefaultNotiticationTypes : INotificationTypeProvider<NotificationType>
    {

        public static readonly NotificationType NewTopicsWeb = 
            new NotificationType("New Topics", "Send me a notification for each new topic.", "Web");

        public static readonly NotificationType NewTopicsEmail =
            new NotificationType("New Topics", "Send me a notification for each new topic.", "Email");

        public static readonly NotificationType NewTopicsMobile =
            new NotificationType("New Topics", "Send me a notification for each new topic.", "Mobile");

        public static readonly NotificationType NewRepliesWeb =
            new NotificationType("New Topics", "Send me a notification for each new topic.", "Web");

        public static readonly NotificationType NewRepliesEmail =
            new NotificationType("New Topics", "Send me a notification for each new topic.", "Email");

        public static readonly NotificationType NewRepliesMobile =
            new NotificationType("New Topics", "Send me a notification for each new topic.", "Mobile");


        public IEnumerable<NotificationType> GetNotificationTypes()
        {
            return new[]
            {
                NewTopicsWeb,
                NewTopicsEmail,
                NewTopicsMobile,
                NewRepliesWeb,
                NewRepliesEmail,
                NewRepliesMobile
            };

        }

        public IEnumerable<NotificationType> GetDefaultPermissions()
        {
            throw new NotImplementedException();
        }
    }
}
