using System;

namespace Plato.Badges.Models
{
    
    public class BadgeAwarderContext<TModel> : IBadgeAwarderContext<TModel> where TModel : class
    {

        public IServiceProvider ServiceProvider { get; }

        public BadgeAwarderContext(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

    }

}
