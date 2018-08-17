using System;
using Plato.Internal.Abstractions;

namespace Plato.Discuss.Channels.Models
{
    public class ChannelDetails : Serializable
    {
        public int TotalTopics { get; set; }

        public int TotalReplies { get; set; }

        public LatestEntity LatestEntity { get; set; }  = new LatestEntity();

    }

    public class LatestEntity
    {

        public int Id { get; set; }
        
        public DateTimeOffset? CreatedDate { get; set; }

        public int CreatedUserId { get; set; }

    }

}
