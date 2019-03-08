using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Labels.Stores
{

    public interface ILabelStore<TModel> : IStore<TModel> where TModel : class
    {
        Task<IEnumerable<TModel>> GetByFeatureIdAsync(int featureId);
    }
    
}
