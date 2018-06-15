using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Layout.Notifications
{
 
    public interface INotify
    {

        void Add(NotificationType type, string message);

        IList<Notification> Notifications { get; }

    }

    public class Notify : INotify
    {

        private readonly IList<Notification> _notifications;
        
        public IList<Notification> Notifications => _notifications;

        public void Add(NotificationType type, string message)
        {
            _notifications.Add(new Notification(type, message));
        }

    }
}
