using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;
using Plato.Tags.Models;

namespace Plato.Tags.Repositories
{

    public interface ITagRepository<TModel> : IRepository2<TModel> where TModel : class, ITag
    {

        Task<IEnumerable<TModel>> SelectByFeatureIdAsync(int featureId);

        Task<TModel> SelectByNameAsync(string name);

        Task<TModel> SelectByNameNormalizedAsync(string nameNormalized);

    }

}
