using System;

namespace Plato.Internal.Models.Notifications
{
    
    public class EmailNotification : INotificationType
    {

        public string Name { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Category { get; }
        
        protected EmailNotification()
        {
            this.Category = "Email";
        }

        public EmailNotification(string name) : this()
        {
            // We always need a name
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            this.Name = name;
        }

        public EmailNotification(string name, string title) : this(name)
        {
            this.Title = title;
        }

        public EmailNotification(
            string name,
            string title,
            string description) : this(name, title)
        {
            this.Description = description;
        }
        
    }

}
