using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Labels.Stores
{

    public interface ILabelStore<TModel> : IStore2<TModel> where TModel : class
    {
        Task<IEnumerable<TModel>> GetByFeatureIdAsync(int featureId);
    }
    
}
