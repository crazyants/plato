using System;

namespace Plato.Internal.Models.Notifications
{
 
    public interface INotificationContext<TModel> where TModel : class
    {
      
        TModel Model { get; set; }

        INotification Notification { get; set; }
        
    }
    
    public class NotificationContext<TModel> : INotificationContext<TModel> where TModel : class
    {

        public TModel Model { get; set; }

        public INotification Notification { get; set; }

        //public IServiceProvider ServiceProvider { get; }

        //public NotificationContext(IServiceProvider serviceProvider)
        //{
        //    this.ServiceProvider = serviceProvider;
        //}

    }

}
