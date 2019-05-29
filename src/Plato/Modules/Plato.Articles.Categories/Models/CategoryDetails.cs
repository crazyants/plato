using System;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Articles.Categories.Models
{

    public class CategoryDetails : Serializable
    {
        public int TotalEntities { get; set; }

        public int TotalReplies { get; set; }

        public LastPost LatestEntity { get; set; } = new LastPost();

        public LastPost LatestReply { get; set; } = new LastPost();

        public bool Closed { get; set; }

    }
    
    public class LastPost
    {
        public int Id { get; set; }

        public string Alias { get; set; }

        public ISimpleUser CreatedBy { get; set; } = new SimpleUser();
        
        public DateTimeOffset? CreatedDate { get; set; }

    }

}
