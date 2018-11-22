using System;

namespace Plato.Badges.Models
{
    
    public class BadgeAwarderContext<TModel> : IBadgeAwarderContext<TModel> where TModel : class
    {

        public TModel Model { get; set; } 

        public IBadge Badge { get; set; }
        
        public BadgeAwarderContext(IBadge dadge)
        {
            this.Badge = dadge;
        }

    }

}
