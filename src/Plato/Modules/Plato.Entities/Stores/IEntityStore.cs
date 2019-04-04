using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{

    public interface IEntityStore<TModel> : IStore<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> GetByFeatureIdAsync(int featureId);

        Task<IEnumerable<TModel>> GetParentsByIdAsync(int entityId);

        Task<IEnumerable<TModel>> GetChildrenByIdAsync(int entityId);

    }


}
