using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Layout.Notifications
{

    public enum NotificationType
    {
        Primary,
        Default,
        Info,
        Success,
        Warning,
        Danger
    }


    public class Notification
    {

        public NotificationType Type { get; set; }

        public string Message { get; set; }

        public Notification(NotificationType type, string message)
        {
            this.Type = type;
            this.Message = message;
        }

    }
}
