namespace Plato.Internal.Notifications.Abstractions.Models
{

    public interface INotificationType
    {
        string Id { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        string Category { get; set; }

    }

}
