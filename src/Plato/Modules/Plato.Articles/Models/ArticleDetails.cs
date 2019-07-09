using System.Collections.Generic;
using Plato.Entities.Models;
using Plato.Internal.Abstractions;

namespace Plato.Articles.Models
{

    public class ArticleDetails : Serializable
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
