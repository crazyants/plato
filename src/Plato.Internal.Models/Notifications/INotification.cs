using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Models.Notifications
{

 
    public interface INotification
    {
        
        IUser To { get; set; }

        INotificationType Type { get; }
        
    }

}
