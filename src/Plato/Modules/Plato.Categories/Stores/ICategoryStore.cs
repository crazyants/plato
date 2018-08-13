using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Categories.Stores
{

    public interface ICategoryStore<TModel> : IStore<TModel> where TModel : class
    {
        Task<IEnumerable<TModel>> GetByFeatureIdAsync(int featureId);
        
    }
    
}
