using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Models.Users;

namespace Plato.Discuss.Moderation.Models
{
    public class OldModerator
    {

        public int Id { get; set; }

        public int UserId { get; set; }

        public SimpleUser User { get; set; }

        public int CategoryId { get; set; }

        public bool EditTopics { get; set; }
        
        public bool EditReplies { get; set; }



    }
}
