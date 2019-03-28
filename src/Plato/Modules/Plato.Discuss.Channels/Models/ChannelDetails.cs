using System;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Discuss.Channels.Models
{

    public class ChannelDetails : Serializable
    {
        public int TotalEntities { get; set; }

        public int TotalReplies { get; set; }

        public LatestPost LatestEntity { get; set; } = new LatestPost();

        public LatestPost LatestReply { get; set; } = new LatestPost();

    }
    
    public class LatestPost
    {
        public int Id { get; set; }

        public string Alias { get; set; }

        public ISimpleUser CreatedBy { get; set; } = new SimpleUser();
        
        public DateTimeOffset? CreatedDate { get; set; }

    }

}
