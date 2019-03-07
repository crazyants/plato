using System;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Articles.Categories.Models
{

    public class CategoryDetails : Serializable
    {
        public int TotalTopics { get; set; }

        public int TotalReplies { get; set; }

        public LastPost LastTopic { get; set; } = new LastPost();

        public LastPost LastReply { get; set; } = new LastPost();

    }
    
    public class LastPost
    {
        public int Id { get; set; }

        public string Alias { get; set; }

        public ISimpleUser CreatedBy { get; set; } = new SimpleUser();
        
        public DateTimeOffset? CreatedDate { get; set; }

    }

}
