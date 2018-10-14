using System;

namespace Plato.Badges.Models
{

    public class AwarderContext
    {

        public IBadge Badge { get; set; }

        public IServiceProvider ServiceProvider { get; }

        public AwarderContext(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }
        
    }

}
