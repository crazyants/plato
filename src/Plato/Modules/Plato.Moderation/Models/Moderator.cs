using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Moderation.Models
{
    public class Moderator
    {

        public int Id { get; set; }

        public int UserId { get; set; }

        public int CategoryId { get; set; }

        public List<ModeratorClaim> ModeratorClaims { get; } = new List<ModeratorClaim>();

        public int CreatedUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public int ModifiedUserId { get; set; }

        public DateTimeOffset? ModifiedDate { get; set; }

    }

}
