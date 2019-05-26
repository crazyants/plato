using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;
using Plato.Tags.Models;

namespace Plato.Tags.Stores
{

    public interface ITagStore<TModel> : IStore2<TModel> where TModel : class, ITag
    {

        Task<IEnumerable<TModel>> GetByFeatureIdAsync(int featureId);

        Task<TModel> GetByNameAsync(string name);
        
        Task<TModel> GetByNameNormalizedAsync(string nameNormalized);

    }
    
}
