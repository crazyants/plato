using System;
using Plato.Internal.Models.Users;

namespace Plato.Entities.Models
{

    public class EntityContributor : SimpleUser
    {

        public int ContributionCount { get; set; }

        public DateTimeOffset ContributionDate { get; set; }
        
        public EntityContributor()
        {
        }

        public EntityContributor(IUser user) : base(user)
        {
        }
    
    }

  

}

 
