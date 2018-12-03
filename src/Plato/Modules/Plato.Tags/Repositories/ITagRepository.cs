using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Tags.Repositories
{

    public interface ITagRepository<T> : IRepository<T> where T : class
    {
        Task<IEnumerable<T>> SelectByFeatureIdAsync(int featureId);
        

    }

}
