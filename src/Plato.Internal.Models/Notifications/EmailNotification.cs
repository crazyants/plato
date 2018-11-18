namespace Plato.Internal.Models.Notifications
{
    
    public class EmailNotification : INotificationType
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; }
        
        public EmailNotification()
        {
            this.Category = "Email";
        }

        public EmailNotification(string id) : this()
        {
            this.Id = id;
        }

        public EmailNotification(string id, string name) : this(id)
        {
            this.Name = name;
        }

        public EmailNotification(
            string id,
            string name,
            string description) : this(id, name)
        {
            this.Description = description;
        }
        
    }

}
