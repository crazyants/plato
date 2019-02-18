using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Notifications.Abstractions
{

    public interface INotificationTypeManager
    {
        
        IEnumerable<INotificationType> GetNotificationTypes(IEnumerable<string> roleNames);
        
        IEnumerable<INotificationType> GetDefaultNotificationTypes(IEnumerable<string> roleNames);

        IDictionary<string, IEnumerable<INotificationType>> GetCategorizedNotificationTypes(IEnumerable<string> roleNames);

    }
    
}
