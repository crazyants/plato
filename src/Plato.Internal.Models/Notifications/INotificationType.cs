namespace Plato.Internal.Models.Notifications
{

    public interface INotificationType
    {
        string Name { get; set; }

        string Title { get; set; }

        string Description { get; set; }

        string Category { get; }
        
    }

}
