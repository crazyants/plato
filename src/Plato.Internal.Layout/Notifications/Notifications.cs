using System.Collections.Generic;

namespace Plato.Internal.Layout.Notifications
{
 
    public interface INotify
    {

        void Add(NotifyType type, string message);

        ICollection<Notification> Notifications { get; }

    }

    public class Notify : INotify
    {

        public ICollection<Notification> Notifications { get; set; }

        public void Add(NotifyType type, string message)
        {
            if (Notifications == null)
            {
                Notifications = new List<Notification>();
            }
            Notifications.Add(new Notification(type, message));
        }

    }
}
