using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Badges.Models
{

    public class AwarderContext
    {

        public AwarderContext(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        public IBadge Badge { get; set; }

        public IServiceProvider ServiceProvider { get; }
        
    }

}
