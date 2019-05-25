using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Labels.Stores
{
    public interface IEntityLabelStore<TModel> : IStore2<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> GetByEntityIdAsync(int entityId);

        Task<bool> DeleteByEntityIdAsync(int entityId);

        Task<bool> DeleteByEntityIdAndLabelIdAsync(int entityId, int LabelId);

    }

}
