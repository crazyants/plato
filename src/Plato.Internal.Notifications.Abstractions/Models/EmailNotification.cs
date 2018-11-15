using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Notifications.Abstractions.Models
{
    public class EmailNotification : INotificationType
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }
        public EmailNotification()
        {
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

        public EmailNotification(
            string id,
            string name,
            string description,
            string category) : this(id, name, description)
        {
            this.Category = category;
        }


    }

    public class WebNotification : INotificationType
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }
        public WebNotification()
        {
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
            string category) : this(id, name, description)
        {
            this.Category = category;
        }


    }


}
