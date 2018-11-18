namespace Plato.Internal.Models.Notifications
{

    public class WebNotification : INotificationType
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; }

        public WebNotification()
        {
            this.Category = "Web";
        }

        public WebNotification(string id) : this()
        {
            this.Id = id;
        }

        public WebNotification(string id, string name) : this(id)
        {
            this.Name = name;
        }

        public WebNotification(
            string id,
            string name,
            string description) : this(id, name)
        {
            this.Description = description;
        }


    }

}
