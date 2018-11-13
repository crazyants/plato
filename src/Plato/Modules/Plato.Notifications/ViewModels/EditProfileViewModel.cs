using System.Collections.Generic;
using Plato.Notifications.Models;

namespace Plato.Notifications.ViewModels
{

    public class EditNotificationsViewModel
    {

        public int Id { get; set; }

        public IDictionary<string, IEnumerable<NotificationType>> CategorizedNotificationTypes { get; set; }

        public IEnumerable<string> EnabledNotificationTypes { get; set; }
        
    }

}
