using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Discuss.Moderation.Models
{
    public class Moderator
    {

        public int Id { get; set; }

        public int UserId { get; set; }

        public int CategoryId { get; set; }

        public bool EditTopics { get; set; }
        
        public bool EditReplies { get; set; }



    }
}
