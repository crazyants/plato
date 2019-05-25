using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Entities.Repositories
{
    public interface IEntityRepository<T> : IRepository2<T> where T : class
    {

        Task<IEnumerable<T>> SelectByFeatureIdAsync(int featureId);

    }

}
