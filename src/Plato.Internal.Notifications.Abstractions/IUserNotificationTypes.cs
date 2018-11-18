using System.Collections.Generic;

namespace Plato.Internal.Notifications.Abstractions
{

    public interface IUserNotificationType
    {
        string Id { get; set; }
    }
    
    public interface IUserNotificationTypes
    {
        IEnumerable<IUserNotificationType> NotificationTypes { get; set; }
    }


}
