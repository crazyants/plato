using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Notifications.Models;

namespace Plato.Notifications.ViewModels
{

    public class EditNotificationsViewModel
    {

        public int Id { get; set; }

        public IDictionary<string, IEnumerable<INotificationType>> CategorizedNotificationTypes { get; set; }

        public IEnumerable<IUserNotificationType> EnabledNotificationTypes { get; set; }
        
    }

}
