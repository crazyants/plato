using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Models.Notifications
{
    
    public class Notification : INotification
    {
   
        public IUser To { get; set; }
 
        public INotificationType Type { get; set; }

    }

}
