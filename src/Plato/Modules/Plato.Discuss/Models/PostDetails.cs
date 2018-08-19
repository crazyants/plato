using System;
using System.Collections.Generic;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Discuss.Models
{

    public class PostDetails : Serializable
    {
     
        public SimpleReply LatestReply { get; set; } = new SimpleReply();

        public IEnumerable<SimpleReply> LatestReplies { get; set; } = new List<SimpleReply>();
        
    }

    public class SimpleReply
    {
        public int Id { get; set; }

        public SimpleUser CreatedBy { get; set; } = new SimpleUser();

        public DateTimeOffset? CreatedDate { get; set; }
    }

}
