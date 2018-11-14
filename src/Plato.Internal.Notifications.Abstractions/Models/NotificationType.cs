namespace Plato.Internal.Notifications.Abstractions.Models
{
    
    public class NotificationType : INotificationType
    {

        public string Id{ get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }
        
        public NotificationType() 
        {
        }

        public NotificationType(string id) : this()
        {
            this.Id = id;
        }

        public NotificationType(string id, string name) : this(id)
        {
            this.Name = name;
        }

        public NotificationType(
            string id,
            string name,
            string description) : this(id, name)
        {
            this.Description = description;
        }

        public NotificationType(
            string id,
            string name,
            string description,
            string category) : this(id, name, description)
        {
            this.Category = category;
        }
        
    }
    
}
