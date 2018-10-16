using System;

namespace Plato.Badges.Models
{

    public interface IBadgeAwarderContext
    {
        IBadge Badge { get; set; }

        IServiceProvider ServiceProvider { get; }

    }

    public class BadgeAwarderContext : IBadgeAwarderContext
    {

        public IBadge Badge { get; set; }

        public IServiceProvider ServiceProvider { get; }

        public BadgeAwarderContext(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }
        
    }

}
