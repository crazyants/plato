using System;
using System.Collections.Generic;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Notifications.Abstractions.Models;

namespace Plato.Mentions
{
    public class Notitications : INotificationTypeProvider<NotificationType>
    {
        
        public static readonly NotificationType NewMentionWeb =
            new NotificationType("NewMentionWeb", "New Mentions", "Show me a web notification for each new @mention.", "Web");

        public static readonly NotificationType NewMentionEmail =
            new NotificationType("NewMentionEmail", "New Mentions", "Send me a email notification for each @mention.", "Email");

        public IEnumerable<NotificationType> GetNotificationTypes()
        {
            return new[]
            {
                NewMentionWeb,
                NewMentionEmail
            };

        }

        public IEnumerable<NotificationType> GetDefaultPermissions()
        {
            throw new NotImplementedException();
        }
    }
}
