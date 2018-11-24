using System;

namespace Plato.Internal.Models.Notifications
{
 
    public interface INotificationContext<TModel> where TModel : class
    {
        INotification Notification { get; set; }

        TModel Model { get; set; }
        
        //IServiceProvider ServiceProvider { get; }

    }
    
    public class NotificationContext<TModel> : INotificationContext<TModel> where TModel : class
    {

        public INotification Notification { get; set; }

        public TModel Model { get; set; }
        
        //public IServiceProvider ServiceProvider { get; }
        
        //public NotificationContext(IServiceProvider serviceProvider)
        //{
        //    this.ServiceProvider = serviceProvider;
        //}

    }

}
