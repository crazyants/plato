using System.Collections.Generic;
using Plato.Internal.Models.Users;

namespace Plato.Entities.Ratings.Models
{

    public class RatingEntryGrouped : RatingEntry
    {

        public RatingEntryGrouped(IRatingEntry rating) : base()
        {
            CreatedBy = rating.CreatedBy;
            CreatedDate = rating.CreatedDate;
            this.Users.Add(rating.CreatedBy);
        }
        
        public IList<ISimpleUser> Users { get; } = new List<ISimpleUser>();

    }

}
