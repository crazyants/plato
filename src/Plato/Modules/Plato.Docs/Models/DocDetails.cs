using System.Collections.Generic;
using Plato.Entities.Models;
using Plato.Internal.Abstractions;

namespace Plato.Docs.Models
{

    public class DocDetails : Serializable
    {

        public IList<EntityUser> LatestUsers { get; set; } = new List<EntityUser>();

        public IList<EntityContributor> Contributors { get; set; } = new List<EntityContributor>();
        
        public int TotalContributions
        {
            get
            {
                var i = 0;
                foreach (var contributor in Contributors)
                {
                    i += contributor.ContributionCount;
                }

                return i;
            }
        }

    }
    
}
