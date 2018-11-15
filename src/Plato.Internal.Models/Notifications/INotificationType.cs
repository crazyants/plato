using System;

namespace Plato.Internal.Models.Notifications
{

    public interface INotificationType
    {
        string Id { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        string Category { get; }

        Action<INotificationContext> Sender { get; set; }


    }

}
