using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Entities.Models
{

    public class EntityContributor : SimpleUser
    {
        public IList<EntityContribution> Contributions { get; set; } = new List<EntityContribution>();

        public EntityContributor()
        {
        }

        public EntityContributor(IUser user) : base(user)
        {
        }
    
    }

    public class EntityContribution
    {
        public DateTimeOffset? Date { get; }

        public EntityContribution(DateTimeOffset? date)
        {
            Date = date;
        }
    }

}

 
