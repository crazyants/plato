using System;
using Plato.Internal.Models.Users;

namespace Plato.Entities.Ratings.Models
{

    public interface IRatingEntry 
    {
        ISimpleUser CreatedBy { get; set; }

        DateTimeOffset? CreatedDate { get; set; }

    }

}
