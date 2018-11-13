namespace Plato.Notifications.Models
{
    
    public class NotificationType : INotificationType
    {

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }


        public NotificationType()
        {
        }

        public NotificationType(string name)
        {
            this.Name = name;
        }

        public NotificationType(
            string name,
            string description) : this(name)
        {
            this.Description = description;
        }

        public NotificationType(
            string name,
            string description,
            string category) : this(name, description)
        {
            this.Category = category;
        }
        
    }


}
