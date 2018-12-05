using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Tags.Stores
{

    public interface ITagStore<TModel> : IStore<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> GetByFeatureIdAsync(int featureId);

        Task<TModel> GetByNameAsync(string name);
        
        Task<TModel> GetByNameNormalizedAsync(string name);

    }
    
}
