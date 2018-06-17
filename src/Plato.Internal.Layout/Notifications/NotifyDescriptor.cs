using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Layout.Notifications
{

    public enum NotifyType
    {
        Success,
        Info,
        Warning,
        Danger
    }
    
    public class Notification
    {

        public NotifyType Type { get; set; }

        public string Message { get; set; }

        public Notification(NotifyType type, string message)
        {
            this.Type = type;
            this.Message = message;
        }

    }
}
