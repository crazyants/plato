using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Layout.Notifications
{
 
    public interface INotify
    {

        void Add(NotificationType type, string message);

        ICollection<Notification> Notifications { get; }

    }

    public class Notify : INotify
    {

        public ICollection<Notification> Notifications { get; set; }

        public void Add(NotificationType type, string message)
        {
            if (Notifications == null)
            {
                Notifications = new List<Notification>();
            }
            Notifications.Add(new Notification(type, message));
        }

    }
}
