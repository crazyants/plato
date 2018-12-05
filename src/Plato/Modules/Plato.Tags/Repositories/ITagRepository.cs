using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Tags.Repositories
{

    public interface ITagRepository<T> : IRepository<T> where T : class
    {
        Task<IEnumerable<T>> SelectByFeatureIdAsync(int featureId);

        Task<T> SelectByNameAsync(string name);

        Task<T> SelectByNameNormalizedAsync(string nameNormalized);
    }

}
