using System.Collections.Generic;
using Plato.Internal.Models.Notifications;

namespace Plato.Internal.Notifications.Abstractions
{
    public interface INotificationTypeManager
    {

        IEnumerable<INotificationType> GetNotificationTypes();

        IDictionary<string, IEnumerable<INotificationType>> GetCategorizedNotificationTypes();

    }
    
}
