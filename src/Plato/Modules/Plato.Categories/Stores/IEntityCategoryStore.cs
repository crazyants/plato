using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Categories.Stores
{
    public interface IEntityCategoryStore<TModel> : IStore<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> GetByEntityId(int entityId);

        Task<bool> DeleteByEntityId(int entityId);

        Task<bool> DeleteByEntityIdAndCategoryId(int entityId, int categoryId);

    }

}
