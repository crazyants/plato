using System;
using Plato.Internal.Abstractions;

namespace Plato.Discuss.Channels.Models
{
    public class ChannelDetails : Serializable
    {
        public int TotalTopics { get; set; }

        public int TotalReplies { get; set; }

        public LastPost LastPost { get; set; } = new LastPost();
        
    }

    public class LastPost
    {

        public int EntityId { get; set; }
        
        public int EntityReplyId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public int CreatedUserId { get; set; }

    }

}
