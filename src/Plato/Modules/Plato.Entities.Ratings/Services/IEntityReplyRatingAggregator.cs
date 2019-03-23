using System.Threading.Tasks;
using Plato.Entities.Models;

namespace Plato.Entities.Ratings.Services
{

    public interface IEntityReplyRatingAggregator<TEntityReply> where TEntityReply : class, IEntityReply
    {

        Task<TEntityReply> UpdateAsync(TEntityReply entity);

    }

}
