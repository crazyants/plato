using System.Collections.Generic;
using Plato.Entities.Models;
using Plato.Internal.Abstractions;

namespace Plato.Docs.Models
{

    public class DocDetails : Serializable
    {

        public IEnumerable<EntityUser> LatestUsers { get; set; } = new List<EntityUser>();

        public IEnumerable<EntityUser> Contributors { get; set; } = new List<EntityUser>();

    }
    
}
