namespace Plato.Internal.Models.Notifications
{

    public class UserNotificationType
    {
        public string Name { get; set; }

        public bool Enabled { get; set; }

        public UserNotificationType()
        {
        }
        
        public UserNotificationType(string name)
        {
            Name = name;
            Enabled = true;
        }

        public UserNotificationType(string name, bool enabled) : this(name)
        {
            Enabled = enabled;
        }

    }

}
