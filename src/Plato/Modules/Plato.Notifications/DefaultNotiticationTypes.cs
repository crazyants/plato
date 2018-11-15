using System;
using System.Collections.Generic;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Notifications.Abstractions.Models;

namespace Plato.Notifications
{
    public class DefaultNotiticationTypes : INotificationTypeProvider
    {

        public static readonly NotificationType NewTopicsWeb = 
            new NotificationType("NewTopicsWeb", "New Topics", "Show me a web notification for each new topic.", "Web");

        public static readonly NotificationType NewTopicsEmail =
            new NotificationType("NewTopicsEmail", "New Topics", "Send me a email notification for each new topic.", "Email");

        public static readonly NotificationType NewTopicsMobile =
            new NotificationType("NewTopicsMobile", "New Topics", "Send me a push notification for each new topic.", "Mobile");

        public static readonly NotificationType NewRepliesWeb =
            new NotificationType("NewRepliesWeb", "New Replies", "Show me a web notification for each new topic reply.", "Web");

        public static readonly NotificationType NewRepliesEmail =
            new NotificationType("NewRepliesEmail", "New Replies", "Send me an email notification for each new reply.", "Email");

        public static readonly NotificationType NewRepliesMobile =
            new NotificationType("NewRepliesMobile", "New Replies", "Send me a push notification for each new reply.", "Mobile");
        
        public IEnumerable<INotificationType> GetNotificationTypes()
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

        public IEnumerable<INotificationType> GetDefaultPermissions()
        {
            throw new NotImplementedException();
        }
    }
}
