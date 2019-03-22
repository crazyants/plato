using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Entities.Ratings.Models;

namespace Plato.Entities.Ratings.Stores
{
    public interface ISimpleRatingsStore
    {

        Task<IEnumerable<SimpleRating>> GetSimpleRatingsAsync(int entityId, int entityReplyId);

    }

}