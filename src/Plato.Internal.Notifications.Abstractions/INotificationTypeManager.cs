using System.Collections.Generic;

namespace Plato.Internal.Notifications.Abstractions
{
    public interface INotificationTypeManager<TNotificationType> where TNotificationType : class
    {

        IEnumerable<TNotificationType> GetNotificationTypes();

        IDictionary<string, IEnumerable<TNotificationType>> GetCategorizedNotificationTypes();

    }
    
}
