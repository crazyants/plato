using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Labels.Stores
{
    public interface IEntityLabelStore<TModel> : IStore<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> GetByEntityId(int entityId);

        Task<bool> DeleteByEntityId(int entityId);

        Task<bool> DeleteByEntityIdAndLabelId(int entityId, int LabelId);

    }

}
