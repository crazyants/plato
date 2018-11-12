using System.Collections.Generic;

namespace Plato.Notifications.Services
{
    public interface INotificationTypeManager<TNotificationType> where TNotificationType : class
    {

        IEnumerable<TNotificationType> GetNotificationTypes();

        IDictionary<string, IEnumerable<TNotificationType>> GetCategorizedNotificationTypesAsync();

    }
    
}
