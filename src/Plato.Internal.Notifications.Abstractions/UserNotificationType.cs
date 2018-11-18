namespace Plato.Internal.Notifications.Abstractions
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
