using System.Collections.Generic;

namespace Plato.Internal.Models.Notifications
{
    
    public interface IUserNotificationTypes
    {
        IEnumerable<UserNotificationType> NotificationTypes { get; set; }
    }
    
}
