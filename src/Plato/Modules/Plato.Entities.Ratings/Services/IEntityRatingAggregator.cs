using System.Threading.Tasks;
using Plato.Entities.Models;

namespace Plato.Entities.Ratings.Services
{

    public interface IEntityRatingAggregator<TEntity> where TEntity : class, IEntity
    {

        Task<TEntity> UpdateAsync(TEntity entity);

    }

}
