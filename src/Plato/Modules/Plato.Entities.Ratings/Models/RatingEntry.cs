using System;
using Plato.Internal.Models.Users;

namespace Plato.Entities.Ratings.Models
{
    public class RatingEntry : IRatingEntry
    {

        public ISimpleUser CreatedBy { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

    }

}
