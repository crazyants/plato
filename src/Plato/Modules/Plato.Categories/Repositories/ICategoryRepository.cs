using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Categories.Repositories
{

    public interface ICategoryRepository<T> : IRepository2<T> where T : class
    {
        Task<IEnumerable<T>> SelectByFeatureIdAsync(int featureId);

    }

}
