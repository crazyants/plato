using System.Collections.Generic;
using Plato.Entities.Models;
using Plato.Internal.Abstractions;

namespace Plato.Discuss.Models
{



    public class PostDetails : Serializable
    {

        public string SomeNewValue { get; set; }
        
        public IEnumerable<EntityUser> Participants { get; set; } = new List<EntityUser>();


    }
}
