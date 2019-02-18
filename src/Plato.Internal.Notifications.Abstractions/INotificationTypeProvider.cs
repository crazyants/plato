using System.Collections.Generic;

namespace Plato.Internal.Notifications.Abstractions
{

    public interface INotificationTypeProvider
    {
    
        IEnumerable<DefaultNotificationTypes> GetNotificationTypes();
        
        IEnumerable<DefaultNotificationTypes> GetDefaultNotificationTypes();

    }

}
