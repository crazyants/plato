namespace Plato.Notifications.Models
{

    public interface IUserNotificationType
    {
        string Id { get; set; }
    }

    public class UserNotificationType : IUserNotificationType
    {
        public string Id { get; set; }

        public UserNotificationType(string id)
        {
            this.Id = id;
        }

    }

}
