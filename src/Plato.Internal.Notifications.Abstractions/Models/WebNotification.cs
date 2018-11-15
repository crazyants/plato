using System;

namespace Plato.Internal.Notifications.Abstractions.Models
{
    public class WebNotification : INotificationType
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; }

        public Action<INotificationContext> Sender { get; set; }

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

        public WebNotification(
            string id,
            string name,
            string description,
            Action<INotificationContext> sender) : this(id, name, description)
        {
            this.Sender = sender;
        }

    }

}
