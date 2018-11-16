using Plato.Internal.Models.Users;

namespace Plato.Internal.Models.Notifications
{
    
    public interface INotification
    {
        
        IUser To { get; set; }

        IUser From { get; set; }
        
        INotificationType Type { get; }
        
    }

}
