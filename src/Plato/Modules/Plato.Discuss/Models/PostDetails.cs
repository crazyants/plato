using System.Collections.Generic;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Discuss.Models
{

    public class PostDetails : Serializable
    {

        public string SomeNewValue { get; set; }
        
        public int TotalReplies { get; set; }

        public IEnumerable<SimpleUser> Participants { get; set; } = new List<SimpleUser>();
        
    }
}
