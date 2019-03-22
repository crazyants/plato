using System.Threading.Tasks;
using Plato.Entities.Ratings.Models;

namespace Plato.Entities.Ratings.Services
{
    public interface IEntityRatingsUpdater
    {

        Task<UpdatedRating> UpdateEntityRating(int entityId);

        Task<UpdatedRating> UpdateEntityRating(int entityId, int replyId);

    }
    
}
