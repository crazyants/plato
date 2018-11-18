using Plato.Internal.Notifications.Abstractions;

namespace Plato.Notifications.Models
{
    
    public class UserNotificationType : IUserNotificationType
    {
        public string Id { get; set; }

        public UserNotificationType(string id)
        {
            this.Id = id;
        }

    }

}
